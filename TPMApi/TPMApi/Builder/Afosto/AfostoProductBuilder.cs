using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMApi.Builder.Afosto.Requirements;
using TPMApi.Builder.Afosto.SteigerhouthuisCustom;
using TPMApi.Models;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;
using Options = TPMHelper.AfostoHelper.ProductModel.Options;

namespace TPMApi.Builder.Afosto
{
    public class AfostoProductBuilder : IAfostoProductBuilder, ISteigerhoutCustomOptions
    {
        public List<JArray> AfostoProductRequirements;
        public JToken TaxClass;
        public IOptions<AuthorizationPoco> Config;
        public Product Product;
        public WCObject WCObject;

        Product IAfostoWCRequirements.Product => Product;
        WCObject IAfostoWCRequirements.WCObject => WCObject;

        public AfostoProductBuilder(
            List<JArray> afostoProductRequirements,
            JToken taxClass,
            IOptions<AuthorizationPoco> config,
            Product product,
            WCObject wcObject)
        {
            AfostoProductRequirements = afostoProductRequirements;
            TaxClass = taxClass;
            Config = config;
            Product = product;
            WCObject = wcObject;
        }

        /*--------------- DESCRIPTORS ---------------*/

        /// <summary>
        /// Set Afosto Descriptor data
        /// </summary>
        /// <param name="product"></param>
        /// <param name="config"></param>
        /// <param name="metaGroups"></param>
        /// <returns></returns>
        public List<Descriptors> SetDescriptors()
        {
            Descriptors descriptor = new Descriptors()
            {
                Name = Product.name,
                description = Product.description,
                Short_Description = Product.short_description,
                MetaGroup = AfostoProductRequirements[1],
                Seo = SetSeo(),
            };

            List<Descriptors> descriptors = new List<Descriptors>();
            descriptors.Add(descriptor);

            return descriptors;
        }

        /// <summary>
        /// Set Afosto SEO data.
        /// Part of Description
        /// </summary>
        /// <returns></returns>
        public Seo SetSeo()
        {
            var seo = new Seo()
            {
                Title = Product.slug,
                Description = Product.description,
                Keywords = Product.name,
                robots = ""
            };

            return seo;
        }

        /*--------------- ITEMS ---------------*/

        /// <summary>
        /// Set Afosto Items data, Wich is the Variants variant in WooCommerce
        /// </summary>
        /// <returns></returns>
        public async Task<List<Items>> SetItems()
        {
            int x;
            var items = new List<Items>();
            var variations = await WooProdVariations();

            foreach (var variation in variations)
            {
                if (variation.attributes.Count > 0)
                {
                    var item = new Items()
                    {
                        Ean = EanCheck(variation.sku),
                        Sku = SkuGenerator(variation.id),
                        Inventory = SetInventory(variation),
                        Prices = SetPrices(variation),
                        Options = SetOptions(variation),
                        Suffix = null,
                    };

                    items.Add(item);
                }
                else
                {
                    var notUsedAttributes = AttributesAsVariants();

                    foreach (var attribute in notUsedAttributes)
                    {
                        for (x = 0; x < attribute.options.Count;)
                        {
                            var item = new Items()
                            {
                                Ean = EanCheck(variation.sku),
                                Sku = SkuGenerator(RdmShort()),
                                Inventory = SetInventory(variation),
                                Prices = SetCustomPrices(Product.price),
                                Options = BuildOptions(attribute.name, attribute.options[x]),
                                Suffix = null
                            };

                            items.Add(item);
                            x++;
                        }
                    }
                }
            }

            foreach (var option in BuildCustomOptions())
            { 
                items.Add(option);
            }

            Console.WriteLine();

            return items;
        }

        private List<ProductAttributeLine> AttributesAsVariants()
        {
            var variantsAsAttribute = Product.attributes.Where(x => x.variation == true).ToList();

            return variantsAsAttribute;
        }

        /// <summary>
        /// Set Afosto item inventory for ex. Stock & Warehouses
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        public Inventory SetInventory(Variation variation)
        {
            var inventory = new Inventory()
            {
                Total = variation.stock_quantity ?? 0,
                Warehouses = AfostoProductRequirements[2]
            };

            return inventory;
        }

        public Inventory SetCustomInventory(int total)
        {
            var inventory = new Inventory()
            {
                Total = total,
                Warehouses = AfostoProductRequirements[2]
            };

            return inventory;
        }

        /// <summary>
        /// Set the Afosto item price properties.
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        public List<Prices> SetPrices(Variation variation)
        {
            Prices price = new Prices()
            {
                Price = variation.price,
                PriceGross = PriceGross(variation.price),
                IsEnabled = true,
                TaxClass = TaxClass,
                Price_Group = AfostoProductRequirements[3]
            };

            List<Prices> prices = new List<Prices>();
            prices.Add(price);

            return prices;
        }

        private List<Prices> SetCustomPrices(decimal? price)
        {
            Prices priceModel = new Prices()
            {
                Price = price,
                IsEnabled = true,
                TaxClass = TaxClass,
                Price_Group = AfostoProductRequirements[3]
            };

            List<Prices> prices = new List<Prices>();
            prices.Add(priceModel);

            return prices;
        }

        /// <summary>
        /// Set Afosto items options.
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        public List<Options> SetOptions(Variation variation)
        {
            var options = new List<Options>();

            foreach (var attribute in variation.attributes)
            {
                var option = new Options()
                {
                    Key = attribute.name,
                    Value = attribute.option.Split(",")[0]
                };

                options.Add(option);
            }

            return options;
        }

        private List<Options> BuildOptions(string key, string value)
        {
            var option = new Options()
            {
                Key = key,
                Value = value.Split(",")[0]
            };

            List<Options> options = new List<Options>();
            options.Add(option);

            return options;
        }

        /*--------------- COLLECTIONS ---------------*/

        /// <summary>
        /// Based on choice eighter Migration Collections from Woo to Afosto. 
        /// Or use Default afosto collections. (Option not yet implemented)
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>
        public JArray SetCollections()
        {
            //Currently not used. Enable if we want to Migration collections also
            //var availableWooCollections = await WooCategories(wcObject);
            return AfostoProductRequirements[0];
        }

        /*--------------- SPECIFICATIONS ---------------*/

        /// <summary>
        /// Product specs. for ex. size?
        /// </summary>
        /// <returns></returns>
        public List<Specifications> SetSpecifications()
        {
            var specs = new List<Specifications>();

            var wooSpecs = Product.attributes;
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

        /*--------------- GET Prod. Variations ---------------*/

        /// <summary>
        /// Load available variants for current product.
        /// Has to be loaded directly from WCObject.
        /// </summary>
        /// <param name="wcObject"></param>
        /// <returns></returns>
        public async Task<List<Variation>> WooProdVariations()
        {
            var variations = await WCObject.Product.Variations.GetAll(Product.id,
                new Dictionary<string, string>()
                {
                    { "per_page", "50"}
                });

            return variations;
        }

        /*--------------- GET Available WC Categories ---------------*/

        /// <summary>
        /// Load available categories aka collections from WCOjbect.
        /// Has to be loaded directly from WCObject.
        /// </summary>
        /// <param name="wcObject"></param>
        /// <returns></returns>
        public async Task<List<ProductCategory>> WooCategories()
        {
            var wcCollections = await WCObject.Category.GetAll(
                new Dictionary<string, string>()
                {
                    { "per_page", "100"}
                });

            return wcCollections;
        }

        /*--------------- HELPERS ---------------*/

        /// <summary>
        /// Check if nullable EAN is not null else we creat custom EAN.
        /// </summary>
        /// <param name="sourceEan"></param>
        /// <returns></returns>
        public string EanCheck(string sourceEan)
        {
            if (string.IsNullOrEmpty(sourceEan))
            {
                var ean = Convert.ToInt64(Rdm());
                return ean.ToString();
            }

            return sourceEan;
        }

        /// <summary>
        /// Generate Custom Random EAN based on 13 Characters as recommende by afosto.
        /// </summary>
        /// <returns></returns>
        public string Rdm()
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

        private int? RdmShort()
        {
            Random generator = new Random();
            String numb = generator.Next(0, 1000000).ToString("D8");

            if (numb.Distinct().Count() == 1)
            {
                numb = Rdm();
            }

            return Int32.Parse(numb);
        }

        /// <summary>
        /// Generate SKU based on product title + Recource ID.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string SkuGenerator(int? id)
        {
            var stringBuilder = new StringBuilder();
            var words = Product.name.Split(new char[] { '-', ' ' });

            for (var i = 0; i < words.Count(); i++)
            {
                stringBuilder.Append(words[i]);
            }

            return stringBuilder + id.ToString();
        }

        private decimal? PriceGross(decimal? price)
        {
            var vat = (price / 100) * 21;
            var priceGross = (price - vat);

            return priceGross;
        }

        /*--------------- Steigerhout Project Customs ---------------*/

        public List<Items> BuildCustomOptions()
        {
            var coatingOptions = CustomOptions.NanoCoatingOption();
            var customItems = SetCustomItems(coatingOptions, false);

            return customItems;
        }

        public List<Items> SetCustomItems(IDictionary<string, List<string>> options, bool isWashing)
        {
            List<Items> itemsList = new List<Items>();

            foreach (var coatingItem in options)
            {
                var key = coatingItem.Key;
                foreach (var value in coatingItem.Value)
                {
                    var item = new Items()
                    {
                        Ean = EanCheck(null),
                        Sku = SkuGenerator(RdmShort()),
                        Inventory = SetCustomInventory(0),
                        Prices = SetCustomPrices(FinishTypePriceRange(isWashing)),
                        Options = BuildOptions(key, value),
                        Suffix = null
                    };

                    ItemPriceAdjustment(item);
                    itemsList.Add(item);
                }
            }

            return itemsList;
        }

        public void ItemPriceAdjustment(Items item)
        {
            foreach (var option in item.Options)
            {
                if (option.Value.ToLower().Contains("geen"))
                {
                    foreach (var price in item.Prices)
                    {
                        price.Price = 0;
                    }
                }
            }
        }

        public List<ProductAttributeLine> UnusedAttributes()
        {
            var variantsAsAttributes = Product.attributes.Where(x => x.variation == true && 
                                        x.name.ToLower().Contains("afwerkingen")).ToList();
            
            if (variantsAsAttributes.Count > 0)
            { 
                return variantsAsAttributes;
            }

            return null;
        }

        public decimal? FinishTypePriceRange(bool isWashing)
        {
            foreach (var category in Product.categories)
            {
                if (isWashing)
                {
                    switch (category.id)
                    {
                        default:
                            return 20;
                        case 458:
                            return 40;
                        case 15:
                            return 40;
                    }
                }
                else
                {
                    switch (category.id)
                    {
                        default:
                            return 30;
                        case 458:
                            return 50;
                    }
                }
            }

            return null;
        }
    }
}
