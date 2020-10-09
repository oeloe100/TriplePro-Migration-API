﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using TPMApi.Builder.Afosto;
using TPMApi.Builder.Afosto.WTAMapping;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMApi.Services;
using TPMDataLibrary.BusinessLogic;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using WooCommerceNET.WooCommerce.v3.Extension;

namespace TPMApi.Controllers
{
    public class MigrationController : Controller
    {
        private static IOptions<AuthorizationPoco> _config;
        private static ILogger<MigrationController> _logger;
        private static IWebHostEnvironment _env;
        private static AfostoMigrationModelBuilder _wtaMapping;

        private static WooCommerceNET.WooCommerce.Legacy.WCObject _wcObjectLegacy;
        private static WCObject _wcObject;
        private static RestAPI _wcRestAPI;
        private static WooAccessModel _wooAccessModel;

        private static readonly int _pageSize = 100;

        public MigrationController(
            IOptions<AuthorizationPoco> config,
            ILogger<MigrationController> logger,
            IWebHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
            _wtaMapping = new AfostoMigrationModelBuilder(config, logger);

            //Get for ex. Accesstoken etc. from Db.
            _wooAccessModel = WooDataProcessor.GetLastAccessData()[0];
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
        public async Task<ContentResult> StartWTAMigration()
        {
            var index = 0;

            try
            {
                //Remove in product FOR TESTING
                List<JObject> migrationModels = new List<JObject>();

                //Get WooCommerce Shop product count trough Legacy WCObject
                var productCount = await _wcObjectLegacy.GetProductCount();
                //Calculate the amount of pages available
                var pageCount = (int)Math.Ceiling((double)productCount / _pageSize);

                for (var i = 1; i <= pageCount;)
                {
                    //Get Product from WooCOmmerce Rest Api based on page/product count
                    var wcProductList = await GetWCProducts(i, _pageSize);

                    foreach (var wcProduct in wcProductList)
                    {
                        index++;

                        //We first upload the images to afosto. Then we use the given ID to connect product to image.
                        var imageResult = await MigrationMiddelware.UploadImageToAfosto(
                                AfostoDataProcessor.GetLastAccessToken()[0], 
                                _wcRestAPI, _env, wcProduct.images);

                        IAfostoProductBuilder afostoProductBuilder = new AfostoProductBuilder(
                            await Requirements(), await GetTaxClass(),
                            _config, wcProduct, _wcObject, imageResult);

                        //Build the actual model we post to afosto
                        var mappingData = await _wtaMapping.BuildAfostoMigrationModel(afostoProductBuilder);

                        //Post the model we build to Afosto as Json
                        await MigrationMiddelware.BuildWTAMappingModel(
                            AfostoDataProcessor.GetLastAccessToken()[0], 
                            mappingData, _config, _logger, index, wcProduct.id);

                        //Remove in Production only for TESTING
                        migrationModels.Add(mappingData);

                        Console.WriteLine();
                    }

                   i++;
                }

                return OnMigrationCompleted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
                return OnMigrationFailed(ex);
            }
        }

        private ContentResult OnMigrationCompleted()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = "<i class='fas fa-check fa-10x fa-check-custom'></i>"
            };
        }

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
            var taxClass = await LoadAfostoData("/taxclasses");
            return taxClass[0];
        }

        /*--------------- GET Once! ---------------*/

        /// <summary>
        /// Load Data from afosto Rest API based on Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal async Task<JArray> GetDataByPath(string path)
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