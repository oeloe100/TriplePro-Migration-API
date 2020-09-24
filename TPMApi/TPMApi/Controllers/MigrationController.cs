using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPMApi.Mapping.WTAMapping;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMApi.Services;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private static IOptions<AuthorizationPoco> _config;
        private static WTAMapping _wtaMapping;
        private static WCObject _wcObject;

        public MigrationController(IOptions<AuthorizationPoco> config)
        {
            _config = config;
            _wtaMapping = new WTAMapping();
        }

        public async Task Start()
        {
            //properties now emtpy strings. Need to get data from Db. once implemented
            _wcObject = WooConnect.WcObject(
                        "",
                        "");

            //get 10 products from first page woocommerce.
            var wcProductList = await GetWCProducts(1, 10);

            //get metadata from afosto api. Null should be tPoco (afosto tokens poco). 
            //get data from db and set to model
            var afostoMetaData = await MigrationMiddelware.GetAfostoMetaData(
                null, _config, "/metagroups");

            //build the actual product model and POST to afosto api
            //same here what is null should be tPoco.
            await MigrationMiddelware.BuildWTAMappingModel(
                _wtaMapping.MappingData(wcProductList),
                _config, null);
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