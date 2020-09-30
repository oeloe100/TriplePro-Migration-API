using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private static int _productsPerPage = 2;

        public MigrationController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
            _wtaMapping = new WTAMapping(config);

            //Get for ex. Accesstoken etc. from Db.
            var wooAccessData = WooDataProcessor.GetLastAccessData()[0];

            //Connect to WooCommerce Service using LegacyObject >
            //Only used to get product count.
            _wcObjectLegacy = WooConnect.LegacyWcObject(
                wooAccessData.WooClientKey,
                wooAccessData.WooClientSecret);

            //Connect to woocommerce service with latest V3 Wordpress Rest api.
            _wcObject = WooConnect.WcObject(
                wooAccessData.WooClientKey,
                wooAccessData.WooClientSecret);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task Start()
        {
            try
            {
                var productCount = await _wcObjectLegacy.GetProductCount();
                var pageCount = (productCount / _productsPerPage);

                for (var i = 1; i <= pageCount;)
                {
                    var wcProductList = await GetWCProducts(i, _productsPerPage);

                    foreach (var product in wcProductList)
                    { 
                        var mappingData = await _wtaMapping.MappingData(product, _wcObject);

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

        private async Task<List<Product>> GetWCProducts(int page, int productPerPage)
        {
            var wcProducts = await _wcObject.Product.GetAll(new Dictionary<string, string>()
            {
                { "per_page", productPerPage.ToString()},
                { "page", page.ToString()}
            });

            return wcProducts;
        }
    }
}
 