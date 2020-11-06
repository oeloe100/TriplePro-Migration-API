using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TPMApi.Builder.Afosto.Requirements;
using TPMApi.Customs.SteigerhouthuisCustom;
using TPMApi.Helpers;
using TPMApi.Models;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;
using Options = TPMHelper.AfostoHelper.ProductModel.Options;
using Product = WooCommerceNET.WooCommerce.v3.Product;
using Variation = WooCommerceNET.WooCommerce.v3.Variation;
using WCObject = WooCommerceNET.WooCommerce.v3.WCObject;

namespace TPMApi.Builder.Afosto
{
    public class AfostoProductBuilder : IAfostoProductBuilder
    {
        public List<JArray> AfostoProductRequirements;
        public JToken TaxClass;
        public IOptions<AuthorizationPoco> Config;
        public Product Product;
        public WCObject WCObject;
        public List<AfostoImageModelAfterUpload> ImageResult;

        Product IAfostoWCRequirements.Product => Product;
        WCObject IAfostoWCRequirements.WCObject => WCObject;

        private readonly ISteigerhoutCustomOptionsBuilder _steigerhoutCustomOptionsBuilder;
        private List<JObject> _wooCategoriesFromAfosto;
        private List<long> _usedIds;

        public AfostoProductBuilder(
            List<JArray> afostoProductRequirements,
            JToken taxClass,
            List<JObject> wooCategoriesFromAfosto,
            ISteigerhoutCustomOptionsBuilder steigerhoutCustomOptionsBuilder,
            IOptions<AuthorizationPoco> config,
            Product product,
            WCObject wcObject,
            List<AfostoImageModelAfterUpload> imageResult,
            List<long> usedIds)
        {
            AfostoProductRequirements = afostoProductRequirements;
            _steigerhoutCustomOptionsBuilder = steigerhoutCustomOptionsBuilder;
            _wooCategoriesFromAfosto = wooCategoriesFromAfosto;
            TaxClass = taxClass;
            Config = config;
            Product = product;
            WCObject = wcObject;
            ImageResult = imageResult;
            _usedIds = usedIds;
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
            var stringBuilder = new StringBuilder();
            foreach (var tag in Product.tags)
            {
                stringBuilder.Append(tag.name + ",");
            }

            var seo = new Seo()
            {
                Title = Product.slug,
                Description = Product.description,
                Keywords = stringBuilder.ToString(),
                robots = ""
            };

            return seo;
        }

        /*--------------- ITEMS ---------------*/

        /// <summary>
        /// Map WooCommerce Variations to Afosto Items.
        /// Items is the Afosto Term for Variations e.g. WooCommerce
        /// </summary>
        /// <returns></returns>
        public async Task<List<Items>> SetItems()
        {
            var items = new List<Items>();
            var variations = await WooProdVariations();
            var active = HasActiveCustoms(items, variations);

            if (!active)
            {
                //Get variation in variations
                for (var i = 0; i < variations.Count; i++)
                {
                    //Check if we have enough attributes to build variations/items
                    if (variations[i].attributes.Count > 0)
                    {
                        //Default way of building items.
                        var item = new Items()
                        {
                            Ean = AfostoProductBuildingHelpers.EAN13Sum(_usedIds),
                            Sku = AfostoProductBuildingHelpers.SKUGenerator(Product, variations[i].id),
                            Inventory = SetInventory(variations[i]),
                            Prices = SetPrices(variations[i]),
                            Options = SetOptions(variations[i]),
                        };

                        items.Add(item);
                    }
                }
            }

            return items;
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
                PriceGross = AfostoProductBuildingHelpers.PriceGross(variation.price),
                IsEnabled = true,
                TaxClass = TaxClass,
                Price_Group = AfostoProductRequirements[3]
            };

            List<Prices> prices = new List<Prices>();
            prices.Add(price);

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

        /// <summary>
        /// Based on choice eighter Migration Collections from Woo to Afosto. 
        /// Or use Default afosto collections.
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>
        public List<Collections> SetCollections()
        {
            List<Collections> collectionsList = new List<Collections>();
            var prodCategories = Product.categories;
            
            for (var i = 0; i < prodCategories.Count; i++)
            {
                var wooCatName = prodCategories[i].name;
                for (var x = 0; x < _wooCategoriesFromAfosto.Count; x++)
                {
                    if (_wooCategoriesFromAfosto[x]["name"].ToString() == wooCatName)
                    {
                        CollectionsBuilder.OriginalCollectionsBuilder(
                            _wooCategoriesFromAfosto, 
                            collectionsList, 
                            x);
                    }

                    CollectionsBuilder.DefaultCollectionsBuilder(
                        AfostoProductRequirements[0],
                        collectionsList);
                }
            }

            return collectionsList;
        }

        public List<Images> SetImages()
        {
            List<Images> productImages = new List<Images>();

            foreach (var image in ImageResult)
            {
                var imageObject = new Images()
                {
                    Id = image.Id,
                };

                productImages.Add(imageObject);
            }

            return productImages;
        }

        /// <summary>
        /// Product specs. for ex. size?
        /// </summary>
        /// <returns></returns>
        public List<Specifications> SetSpecifications()
        {
            var specs = new List<Specifications>();
            var attributes = Product.attributes;
            var afostoCharacterLimit = 244;

            foreach (var attribute in attributes)
            {
                if (attribute.variation == false && 
                    attribute.visible == true)
                { 
                    foreach (var option in attribute.options)
                    {
                        if (option.Length > afostoCharacterLimit)
                        {
                            var spec = new Specifications()
                            {
                                Key = attribute.name,
                                Value = option.Substring(0, afostoCharacterLimit)
                            };

                            specs.Add(spec);
                        }
                    }
                }
            }

            return specs;
        }

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

        public bool HasActiveCustoms(
            List<Items> items, 
            List<Variation> variations)
        {
            if (_steigerhoutCustomOptionsBuilder != null)
            {
                _steigerhoutCustomOptionsBuilder.BuildCustomOptions(items, variations);
                return true;
            }

            return false;
        }
    }
}
