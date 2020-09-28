using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMDataLibrary.BusinessLogic;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;
using Options = TPMHelper.AfostoHelper.ProductModel.Options;
using TaxClass = TPMHelper.AfostoHelper.ProductModel.TaxClass;

namespace TPMApi.Mapping.WTAMapping
{
    public class WTAMapping
    {
        private Product _product;
        private WCObject _wcObject;
        private readonly IOptions<AuthorizationPoco> _config;

        public WTAMapping(IOptions<AuthorizationPoco> config)
        {
            _config = config;
        }

        public async Task<List<JObject>> MappingData(
            List<Product> WooProducts,
            WCObject wcObject)
        {
            var productsAsJObject = new List<JObject>();
            _wcObject = wcObject;

            for (var i = 0; i < WooProducts.Count; i++)
            {
                _product = WooProducts[i];

                var products = new AfostoProductPoco()
                {
                    Weight = CheckDecimalProperty(_product.weight),
                    Cost = CheckDecimalProperty(_product.price),
                    Is_Backorder_Allowed = _product.backorders_allowed,
                    Is_Tracking_Inventory = false,
                    Descriptors = await SetDescriptors(),
                    Items = await SetItems(i),
                    Images = null,//SetImages(),
                    Specifications = SetSpecifications(),
                };

                var ProductAsjsonString = JsonConvert.SerializeObject(products);
                JObject productAsJsonObject = JObject.Parse(ProductAsjsonString);

                productsAsJObject.Add(productAsJsonObject);
            }

            return productsAsJObject;
        }

        //Descriptors POSTING successfully with 200.
        private async Task<List<Descriptors>> SetDescriptors()
        {
            var descriptors = new List<Descriptors>();

            var descriptor = new Descriptors()
            {
                description = _product.description,
                Name = _product.name,
                Seo = SetSeo(),
                MetaGroup = await SetMetaGroup(),
                Short_Description = _product.short_description
            };

            descriptors.Add(descriptor);

            return descriptors;
        }

        //TODO: Seo Keywords are being POSTED as single word. 
        //Split keywords by ex. (space) and post as single words.
        private Seo SetSeo()
        {
            var seo = new Seo()
            {
                Title = _product.slug,
                Description = _product.description,
                Keywords = _product.name,
                robots = ""
            };

            return seo;
        }

        //Working great. POST with 200 success. 
        private async Task<JToken> SetMetaGroup()
        {
            var afostoMetaData = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken()[0],
                _config, "/metagroups");

            JArray jArrayMetaData = JArray.Parse(afostoMetaData);

            foreach (var metaItem in jArrayMetaData)
            {
                return metaItem;
            }

            return null;
        }

        private async Task<List<Items>> SetItems(int index)
        {
            var items = new List<Items>();

            foreach (var variation in await WooProdVariations())
            { 
                var item = new Items()
                {
                    Ean = Convert.ToInt64(DateTime.Now.Ticks.ToString().Substring(0, 13)),
                    Sku = SkuCreator(variation.id),
                    Cost = _product.regular_price,
                    Inventory = await SetInventory(variation),
                    Prices = await SetPrices(variation),
                    Options = null,
                    Suffix = null,
                };

                items.Add(item);
            }

            return items;
        }

        private async Task<List<Variation>> WooProdVariations()
        {
            var test = _product.id;

            var variations = await _wcObject.Product.Variations.GetAll(_product.id, 
                new Dictionary<string, string>()
                {
                    { "per_page", "50"}
                });

            return variations;
        }

        //Create inventory with default 50 products.
        //amount is in this case not interesting. since we dont track inventory.
        private async Task<Inventory> SetInventory(Variation var)
        {
            var inventory = new Inventory()
            {
                Total = CheckIntProperty(var.stock_quantity),
                Warehouses = await SetWareHouses()
            };

            return inventory;
        }

        //Get first warehouse we can find. and use that. 
        //If multiple we use multiple warehouses.
        private async Task<JArray> SetWareHouses()
        {
            var afostoWarehouses = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken()[0],
                _config, "/warehouses");

            JArray jArrayWarehouses = JArray.Parse(afostoWarehouses);

            return jArrayWarehouses;
        }

        private async Task<List<Prices>> SetPrices(Variation var)
        {
            var prices = new List<Prices>();

            try
            { 
                var price = new Prices()
                {
                    IsEnabled = true,
                    PriceGross = var.price,
                    Price_Group = await GetPriceGroup(),
                    TaxClass = await GetTaxClass(),
                };

                prices.Add(price);

                return prices;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private async Task<JToken> GetPriceGroup()
        {
            var priceGroups = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken()[0],
                _config, "/pricegroups");

            JArray priceGroupsAsJArray = JArray.Parse(priceGroups);

            return priceGroupsAsJArray[0];
        }

        private async Task<JToken> GetTaxClass()
        {
            var taxClasses = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken()[0],
                _config, "/taxclasses");

            JArray taxClassesAsJArray = JArray.Parse(taxClasses);

            return taxClassesAsJArray[0];
        }

        //Response 400 Bad request with message: “Image cannot be blank.”
        //Issue currently pending with afosto. Waiting for response.
        private List<Images> SetImages()
        {
            var images = new List<Images>();

            var wooImages = _product.images;
            foreach (var wooImage in wooImages)
            {
                var image = new Images()
                {
                    IsDefault = false,
                    Label = wooImage.name,
                    Url = wooImage.src
                };

                images.Add(image);
            }

            images.FirstOrDefault<Images>().IsDefault = true;

            return images;
        }

        //DONE posting with 200 success. Seems like its not being saved by Afosto.
        //Issue currently pending with afosto. Waiting for response.
        private List<Specifications> SetSpecifications()
        {
            var specs = new List<Specifications>();

            var wooSpecs = _product.attributes;
            foreach (var wooSpec in wooSpecs)
            {
                foreach (var option in wooSpec.options)
                {
                    var spec = new Specifications()
                    {
                        Key = wooSpec.name,
                        Value = option
                    };

                    specs.Add(spec);
                }
            }

            return specs;
        }

        /*--------------- HELPER METHODS ---------------*/

        private string SkuCreator(int? index)
        {
            var stringBuilder = new StringBuilder();
            var words = _product.name.Split(new char[] { '-', ' ' });
            
            for (var i = 0; i < words.Count(); i++)
            {
                stringBuilder.Append(words[i]);
            }

            return stringBuilder + index.ToString();
        }

        private decimal? CheckDecimalProperty(decimal? prop)
        {
            if (prop == null)
            {
                return 0;
            }

            return prop;
        }

        private int? CheckIntProperty(int? prop)
        {
            if (prop == null)
            {
                return 0;
            }

            return prop;
        }
    }
}
