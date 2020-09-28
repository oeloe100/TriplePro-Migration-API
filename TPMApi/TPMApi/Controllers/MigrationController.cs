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
        private static WCObject _wcObject;

        public MigrationController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
            _wtaMapping = new WTAMapping(config);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task Start()
        {
            var wooAccessData = WooDataProcessor.GetLastAccessData()[0];

            _wcObject = WooConnect.WcObject(
                        wooAccessData.WooClientKey,
                        wooAccessData.WooClientSecret);

            var wcProductList = await GetWCProducts(1, 10);

            /*----------------------- AFOSTO -----------------------*/

            await MigrationMiddelware.BuildWTAMappingModel(
                AfostoDataProcessor.GetLastAccessToken()[0],
                await _wtaMapping.MappingData(wcProductList, 
                _wcObject),
                _config);
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
 