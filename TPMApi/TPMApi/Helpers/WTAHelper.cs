using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
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

namespace TPMApi.Helpers
{
    public class WTAHelper
    {
        private IOptions<AuthorizationPoco> _config;
        private Product _product;

        /*--------------- DESCRIPTORS ---------------*/

        internal List<Descriptors> SetDescriptors(
            Product product, 
            IOptions<AuthorizationPoco> config,
            JArray metaGroups)
        {
            _config = config;
            _product = product;

            var descriptors = new List<Descriptors>();
            var descriptor = new Descriptors()
            {
                description = product.description,
                Name = product.name,
                Seo = SetSeo(),
                MetaGroup = metaGroups,
                Short_Description = product.short_description
            };

            descriptors.Add(descriptor);

            return descriptors;
        }

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

        /*--------------- ITEMS ---------------*/

        internal async Task<List<Items>> SetItems(
            WCObject wcObject,
            JArray warehouses,
            JArray priceGroups,
            JToken taxClass)
        {
            var wooProdVariations = await WooProdVariations(wcObject);
            
            var items = new List<Items>();

            for (var i = 0; i < wooProdVariations.Count; i++)
            { 
                var item = new Items()
                {
                    Ean = SimpleEanGenerator(),
                    Sku = SkuGenerator(wooProdVariations[i].id),
                    Inventory = SetInventory(wooProdVariations[i], warehouses),
                    Prices = SetPrices(wooProdVariations[i], priceGroups, taxClass),
                    Options = SetOptions(wooProdVariations[i]),
                    Suffix = null,
                    Cost = 0
                };

                items.Add(item);
            }

            return items;
        }

        private async Task<List<Variation>> WooProdVariations(WCObject wcObject)
        {
            var variations = await wcObject.Product.Variations.GetAll(_product.id,
                new Dictionary<string, string>()
                {
                    { "per_page", "50"}
                });

            return variations;
        }

        private Inventory SetInventory(
            Variation var,
            JArray warehouses)
        {
            var inventory = new Inventory()
            {
                Total = CheckIntProperty(var.stock_quantity),
                Warehouses = warehouses
            };

            return inventory;
        }

        private List<Prices> SetPrices(
            Variation var,
            JArray priceGroups,
            JToken taxClass)
        {
            var warehouses = new List<Prices>();
            var warehouse = new Prices()
            {
                Price = var.price,
                PriceGross = var.regular_price,
                OriginalPrice = var.price,
                OriginalPriceGross = var.regular_price,
                IsEnabled = true,
                TaxClass = taxClass,
                Price_Group = priceGroups
            };

            warehouses.Add(warehouse);

            return warehouses;
        }

        private List<Options> SetOptions(Variation var)
        {
            var options = new List<Options>();

            foreach (var attribute in var.attributes)
            {
                var option = new Options()
                {
                    Key = attribute.name,
                    Value = attribute.option
                };

                options.Add(option);
            }

            return options;
        }

        /*--------------- COLLECTIONS ---------------*/

        internal JArray SetCollections(JArray collections)
        {
            //Right now we return collections that exist in the shop.
            //Future we need to add to collections as exist in woocommerce >
            //Once we have those collections in afosto
            return collections;
        }

        /*--------------- SPECIFICATIONS ---------------*/

        internal List<Specifications> SetSpecifications()
        {
            var specs = new List<Specifications>();

            var wooSpecs = _product.attributes;
            foreach (var wooSpec in wooSpecs)
            {
                foreach (var option in wooSpec.options)
                {
                    var spec = new Specifications()
                    {
                        Key = wooSpec.name.Split(".")[0],
                        Value = option
                    };

                    specs.Add(spec);
                }
            }

            return specs;
        }

        /*--------------- HELPERS ---------------*/

        private long? SimpleEanGenerator()
        {
            var ean = Convert.ToInt64(Rdm());
            return ean;
        }

        private string Rdm()
        {
            Random generator = new Random();
            String numb = generator.Next(0, 1000000).ToString("D6");
            numb += generator.Next(0, 10000000).ToString("D7");

            if (numb.Distinct().Count() == 1)
            {
                numb = Rdm();
            }

            return numb;
        }

        private string SkuGenerator(int? index)
        {
            var stringBuilder = new StringBuilder();
            var words = _product.name.Split(new char[] { '-', ' ' });

            for (var i = 0; i < words.Count(); i++)
            {
                stringBuilder.Append(words[i]);
            }

            return stringBuilder + index.ToString();
        }

        internal decimal? CheckDecimalProperty(decimal? prop)
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
