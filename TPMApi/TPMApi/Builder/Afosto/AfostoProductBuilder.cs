﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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

        //private readonly ISteigerhoutCustomOptions _steigerhoutCustomOptions;
        private readonly ISteigerhoutCustomOptionsBuilder _steigerhoutCustomOptionsBuilder;

        public AfostoProductBuilder(
            List<JArray> afostoProductRequirements,
            JToken taxClass,
            //ISteigerhoutCustomOptions steigerhoutCustomOptions,
            ISteigerhoutCustomOptionsBuilder steigerhoutCustomOptionsBuilder,
            IOptions<AuthorizationPoco> config,
            Product product,
            WCObject wcObject,
            List<AfostoImageModelAfterUpload> imageResult)
        {
            AfostoProductRequirements = afostoProductRequirements;
            //_steigerhoutCustomOptions = steigerhoutCustomOptions;
            _steigerhoutCustomOptionsBuilder = steigerhoutCustomOptionsBuilder;
            TaxClass = taxClass;
            Config = config;
            Product = product;
            WCObject = wcObject;
            ImageResult = imageResult;
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
        /// Map WooCommerce Variations to Afosto Items.
        /// Items is the Afosto Term for Variations e.g. WooCommerce
        /// </summary>
        /// <returns></returns>
        public async Task<List<Items>> SetItems()
        {
            var items = new List<Items>();
            var variations = await WooProdVariations();

            if (_steigerhoutCustomOptionsBuilder != null)
            { 
                _steigerhoutCustomOptionsBuilder.BuildCustomOptions(items, variations);
            }
            else
            { 
                //Get variation in variations
                foreach (var variation in variations)
                {
                    //Check if we have enough attributes to build variations/items
                    if (variation.attributes.Count > 0)
                    {
                        //Default way of building items.
                        var item = new Items()
                        {
                            Ean = AfostoProductBuildingHelpers.EanCheck(variation.sku),
                            Sku = AfostoProductBuildingHelpers.SKUGenerator(Product, variation.id),
                            Inventory = SetInventory(variation),
                            Prices = SetPrices(variation),
                            Options = SetOptions(variation),
                            Suffix = null,
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
    }
}
