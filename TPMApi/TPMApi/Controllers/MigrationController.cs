using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TPMApi.Builder.Afosto;
using TPMApi.Builder.Afosto.WTAMapping;
using TPMApi.Customs.SteigerhouthuisCustom;
using TPMApi.Helpers;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMApi.Services;
using TPMDataLibrary.BusinessLogic;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Controllers
{
    [Authorize]
    public class MigrationController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;
        private static ILogger<MigrationController> _logger;
        private static IWebHostEnvironment _env;
        private static IEmailService _emailService;
        private static ISteigerhoutCustomOptionsBuilder _steigerhoutCustomOptionsBuilder;
        private static AfostoMigrationModelBuilder _wtaMapping;
        private static UserManager<IdentityUser> _userManager;

        private static WooCommerceNET.WooCommerce.Legacy.WCObject _wcObjectLegacy;
        private static WCObject _wcObject;
        private static RestAPI _wcRestAPI;
        private static WooAccessModel _wooAccessModel;

        private static readonly int _pageSize = 100;
        private static List<long> _usedIds;

        private static SqlConnection _sqlConn;

        public MigrationController(
            IOptions<AuthorizationPoco> config,
            ILogger<MigrationController> logger,
            IWebHostEnvironment env,
            IEmailService emailService,
            UserManager<IdentityUser> userManager)
        {
            _config = config;
            _logger = logger;
            _env = env;
            _emailService = emailService;
            _wtaMapping = new AfostoMigrationModelBuilder(config, logger);
            _userManager = userManager;
            _usedIds = new List<long>();

            //Config string
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            SqlConnection sqlConn = new SqlConnection(builder.
                        GetSection("ConnectionStrings").
                        GetSection("TPMApiContextConnection").Value);

            _sqlConn = sqlConn;

            //Get for ex. Accesstoken etc. from Db.
            _wooAccessModel = WooDataProcessor.GetLastAccessData(sqlConn.ConnectionString)[0];
            CreateWCObjectInstance(_wooAccessModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Start WooCommerce to Afosto migration process.
        /// </summary>
        /// <returns></returns>
        public async Task<ContentResult> StartWTAMigration(List<string> specialsArray)
        {
            //await FailedMigrations();

            try
            {
                var productCount = await CorrectV3ProdCount();
                var pageCount = (int)Math.Ceiling((double)productCount / _pageSize);

                for (var i = 1; i <= pageCount;)
                {
                    foreach (var wcProduct in await GetWCProducts(i, _pageSize))
                    {
                        //First we check if we have included a customOption if so we create an instance.
                        await IncludeCustomOptions(specialsArray, wcProduct);

                        //We first upload the images to afosto. Then we use the given ID to connect product to image.
                        var imageResult = await MigrationMiddelware.UploadImageToAfosto(
                                AfostoDataProcessor.GetLastAccessToken(_sqlConn.ConnectionString)[0],
                                _logger, wcProduct.images);

                        //We build the afosto product model;
                        IAfostoProductBuilder afostoProductBuilder = new AfostoProductBuilder(
                            await Requirements(), await GetTaxClass(), await CategoriesToInclude(),
                            _steigerhoutCustomOptionsBuilder, _config, wcProduct, _wcObject, imageResult, _usedIds);

                        //Build the actual model we post to afosto
                        var mappingData = await _wtaMapping.BuildAfostoMigrationModel(afostoProductBuilder);

                        //Post the model we build to Afosto as Json
                        await MigrationMiddelware.BuildWTAMappingModel(
                            AfostoDataProcessor.GetLastAccessToken(_sqlConn.ConnectionString)[0],
                            mappingData, _config, _logger, wcProduct.id);
                    }

                    i++;
                }

                await FailedMigrations();
                return OnMigrationCompleted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
                return OnMigrationFailed(ex);
            }
        }

        /// <summary>
        /// Check if afosto prod. titles list contains products (published and concept).
        /// If list does not contain title. We log this as a failed migration. Prod. is probably too large or incomplete.
        /// </summary>
        /// <returns></returns>
        private async Task FailedMigrations()
        {
            List<string> failedProdMigrationsByName = new List<string>();

            var productCount = await CorrectV3ProdCount();
            var pageCount = (int)Math.Ceiling((double)productCount / _pageSize);

            var afostoProdTitlesList = await AfostoProductsTitle();

            for (var i = 1; i <= pageCount;)
            {
                var wooProducts = await GetWCProducts(i, _pageSize);
                for (var x = 0; x < wooProducts.Count; x++)
                {
                    if (afostoProdTitlesList.Contains(wooProducts[x].name) == false)
                    {
                        _logger.LogError(wooProducts[x].name);
                        failedProdMigrationsByName.Add(wooProducts[x].name);
                    }
                }

                i++;
            }

            var userInfo = await GetCurrentLoggedinUserInformation();
            string messageBody = string.Join(",", failedProdMigrationsByName);
            _emailService.Send(userInfo?.Email, "Migration Report", messageBody);
        }

        private async Task<List<JObject>> CategoriesToInclude()
        {
            List<JObject> collectionsListAsJObject = new List<JObject>();

            var collectionsCount = await LoadAfostoCountData("/collections/count", 1);
            var pageCount = (int)Math.Ceiling((double)collectionsCount / 50);

            for (var x = 1; x <= pageCount;)
            {
                var collections = await LoadAfostoData("/collections", x);
                for (var i = 0; i < collections.Count; i++)
                {
                    var jObjCollection = JObject.Parse(collections[i].ToString());
                    collectionsListAsJObject.Add(jObjCollection);
                }

                x++;
            }

            return collectionsListAsJObject;
        }

        /// <summary>
        /// Get every single product in afosto after migration.
        /// Collect product titles and conv. to list. > Returns a list of product titles
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> AfostoProductsTitle()
        {
            try
            {
                List<string> afostoProductTitles = new List<string>();

                var productCount = await LoadAfostoCountData("/products/count", 1);
                var pageCount = (int)Math.Ceiling((double)productCount / 50);

                for (var i = 1; i <= pageCount;)
                {
                    var afostoProd = LoadAfostoData("/products", i).Result;
                    for (var x = 0; x < afostoProd.Count; x++)
                    {
                        var descriptors = AfostoProductBuildingHelpers.GetValue<JToken>(afostoProd[x], "descriptors");
                        var name = descriptors[0].GetValue<JToken>("name").ToString();
                        afostoProductTitles.Add(name);
                    }

                    i++;
                }

                return afostoProductTitles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Get Correct product count for Woo V3. Since the count is legacy and published products based.
        /// </summary>
        /// <returns></returns>
        private async Task<int> CorrectV3ProdCount()
        {
            int prodCount = 0;
            var productCount = await _wcObjectLegacy.GetProductCount();
            var pageCount = (int)Math.Ceiling((double)productCount / _pageSize);

            for (var i = 1; i <= pageCount;)
            {
                var x = GetWCProducts(i, _pageSize).Result.Count;
                prodCount += x;
                i++;
            }

            return prodCount;
        }

        /// <summary>
        /// Do we have custom options selected? if so we create instance here and return true;
        /// </summary>
        /// <param name="includedOptions"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private async Task IncludeCustomOptions(List<string> includedOptions, Product product)
        {
            foreach (var option in includedOptions)
            {
                switch (option.ToLower())
                {
                    case "steigerhout":
                        _steigerhoutCustomOptionsBuilder = new SteigerhoutCustomOptionsBuilder(
                        product, await Requirements(), await GetTaxClass(), _usedIds);
                        break;
                }
            }
        }

        /// <summary>
        /// If migration is completed we return html
        /// </summary>
        /// <returns></returns>
        private ContentResult OnMigrationCompleted()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = "<i class='fas fa-check fa-10x fa-check-custom'></i>"
            };
        }

        /// <summary>
        /// if migration has failed we return html
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ContentResult OnMigrationFailed(Exception ex)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = "<i class='fas fa-times'></i> <p>" + ex.Message + "</p>"
            };
        }

        /// <summary>
        /// Get WooCommerce (wcobject V3) Product models
        /// </summary>
        /// <param name="page"></param>
        /// <param name="productPerPage"></param>
        /// <returns></returns>
        private async Task<List<Product>> GetWCProducts(int page, int productPerPage)
        {
            var wcProducts = await _wcObject.Product.GetAll(new Dictionary<string, string>()
            {
                { "per_page", productPerPage.ToString()},
                { "page", page.ToString()}
            });

            return wcProducts;
        }

        private void CreateWCObjectInstance(WooAccessModel wam)
        {
            //Connect to WooCommerce Service using LegacyObject >
            //Only used to get product count.
            _wcObjectLegacy = WooConnect.LegacyWcObject(
                wam.WooClientKey,
                wam.WooClientSecret);

            //Connect to woocommerce service with latest V3 Wordpress Rest api.
            _wcObject = WooConnect.WcObject(
                wam.WooClientKey,
                wam.WooClientSecret);

            _wcRestAPI = WooConnect.WcRestAPI(
                wam.WooClientKey,
                wam.WooClientSecret);
        }

        /// <summary>
        /// Returns A list with required information from Afosto API.
        /// Is loaded once upon calling this controller. Required for building afosto product model(s)
        /// </summary>
        /// <returns></returns>
        public async Task<List<JArray>> Requirements()
        {
            List<JArray> requirementsList = new List<JArray>();

            requirementsList.Add(await GetDataByPath("/collections"));
            requirementsList.Add(await GetDataByPath("/metagroups"));
            requirementsList.Add(await GetDataByPath("/warehouses"));
            requirementsList.Add(await GetDataByPath("/pricegroups"));

            return requirementsList;
        }

        /// <summary>
        /// Load available Afosto taxclasses.
        /// Currently we use the 21% taxclass
        /// </summary>
        /// <returns></returns>
        public async Task<JToken> GetTaxClass()
        {
            var taxClass = await LoadAfostoData("/taxclasses", 1);
            return taxClass[2];
        }

        /// <summary>
        /// Load Data from afosto Rest API based on Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal async Task<JArray> GetDataByPath(string path)
        {
            var data = await LoadAfostoData(path, 1);
            return data;
        }

        /// <summary>
        /// Load the actual afosto data.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task<JArray> LoadAfostoData(string location, int page)
        {
            var reqAfostoData = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken(_sqlConn.ConnectionString)[0],
                _config, location, page);

            JArray jArrayMetaData = JArray.Parse(reqAfostoData);

            return jArrayMetaData;
        }

        /// <summary>
        /// Load afosto product count and convert to int.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<int> LoadAfostoCountData(string location, int page)
        {
            var reqAfostoData = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken(_sqlConn.ConnectionString)[0],
                _config, location, page);

            var convToJObject = JObject.Parse(reqAfostoData)["total"].ToString();
            int count = Int32.Parse(convToJObject);

            return count;
        }

        private async Task<IdentityUser> GetCurrentLoggedinUserInformation()
        {
            var applicationUser = await _userManager.GetUserAsync(User);
            return applicationUser;
        }
    }
}
