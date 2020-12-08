using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TPMApi.Controllers;
using TPMApi.Models;
using TPMHelper.AfostoHelper.ProductModel;

namespace TPMApi.Builder.Afosto.WTAMapping
{
    public class AfostoMigrationModelBuilder
    {
        //Load pre-configured data from appsettings.json 
        public readonly IOptions<AuthorizationPoco> _config;
        private static ILogger<MigrationController> _logger;

        public AfostoMigrationModelBuilder(
            IOptions<AuthorizationPoco> config,
            ILogger<MigrationController> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Build the Afosto product model to POST 
        /// </summary>
        /// <param name="WooProduct"></param>
        /// <param name="wcObject"></param>
        /// <returns></returns>
        public async Task<JObject> BuildAfostoMigrationModel(
            IAfostoProductBuilder afostoProduct)
        {
            try
            {
                var product = new AfostoProductPoco()
                {
                    Weight = afostoProduct.Product.weight ?? 0,
                    Cost = 0,
                    Is_Backorder_Allowed = afostoProduct.Product.backorders_allowed,
                    Is_Tracking_Inventory = false,
                    Descriptors = afostoProduct.SetDescriptors(),
                    Items = await afostoProduct.SetItems(),
                    Collections = afostoProduct.SetCollections(),
                    Specifications = afostoProduct.SetSpecifications(),
                    Images = afostoProduct.SetImages(),
                };

                var ProductAsjsonString = JsonConvert.SerializeObject(product);
                JObject productAsJsonObject = JObject.Parse(ProductAsjsonString);

                return productAsJsonObject;
            }
            catch (Exception ex)
            {
                _logger.LogError("AfostoMigrationModelBuilder Error: " + ex.Message + ex.StackTrace);
                return null;
            }
        }
    }
}
