using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPMApi.Helpers;
using TPMApi.Interfaces;
using TPMApi.Mapping.WTAMapping;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMApi.Services;
using TPMDataLibrary.BusinessLogic;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Controllers
{
    public class MigrationController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;
        private static WTAMapping _wtaMapping;

        private static WooCommerceNET.WooCommerce.Legacy.WCObject _wcObjectLegacy;
        private static WCObject _wcObject;

        private static readonly int _productsPerPage = 2;

        public MigrationController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
            _wtaMapping = new WTAMapping(config);

            //Get for ex. Accesstoken etc. from Db.
            var wooAccessData = WooDataProcessor.GetLastAccessData()[0];
            CreateWCObjectInstance(wooAccessData);
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Start WooCommerce to Afosto migration process.
        /// </summary>
        /// <returns></returns>
        public async Task StartWTAMigration()
        {
            try
            {
                //Get WooCommerce Shop product count trough Legacy WCObject
                var productCount = await _wcObjectLegacy.GetProductCount();
                //Calculate the amount of pages available
                var pageCount = (productCount / _productsPerPage);

                for (var i = 1; i <= pageCount;)
                {
                    //Get Product from WooCOmmerce Rest Api based on page/product count
                    var wcProductList = await GetWCProducts(i, _productsPerPage);

                    foreach (var wcProduct in wcProductList)
                    {
                        IAfostoProductBuilder afostoProductBuilder = new AfostoProductBuilder(
                            await Requirements(), await GetTaxClass(),
                            _config, wcProduct, _wcObject);

                        //Build the actual model we post to afosto
                        var mappingData = await _wtaMapping.BuildAfostoMigrationModel(afostoProductBuilder);

                        //Post the model we build to Afosto as Json
                        await MigrationMiddelware.BuildWTAMappingModel(
                            AfostoDataProcessor.GetLastAccessToken()[0], mappingData, _config);
                    }

                    i++;
                }

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
        }

        /// <summary>
        /// Returns A list with required information from Afosto API.
        /// Is loaded once upon calling this controller. Required for building afosto product model(s)
        /// </summary>
        /// <returns></returns>
        public async Task<List<JArray>> Requirements()
        {
            List<JArray> requirementsList = new List<JArray>();

            requirementsList.Add(await SetAfostoData("/collections"));
            requirementsList.Add(await SetAfostoData("/metagroups"));
            requirementsList.Add(await SetAfostoData("/warehouses"));
            requirementsList.Add(await SetAfostoData("/pricegroups"));

            return requirementsList;
        }

        /// <summary>
        /// Load available Afosto taxclasses.
        /// Currently we use the 21% taxclass
        /// </summary>
        /// <returns></returns>
        public async Task<JToken> GetTaxClass()
        {
            var taxClass = await LoadAfostoData("/taxclasses");
            return taxClass[2];
        }

        /*--------------- GET Once! ---------------*/

        /// <summary>
        /// Load Data from afosto Rest API based on Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal async Task<JArray> SetAfostoData(string path)
        {
            var data = await LoadAfostoData(path);
            return data;
        }

        /// <summary>
        /// Load the actual afosto data.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task<JArray> LoadAfostoData(string location)
        {
            var reqAfostoData = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken()[0],
                _config, location);

            JArray jArrayMetaData = JArray.Parse(reqAfostoData);

            return jArrayMetaData;
        }
    }
}
