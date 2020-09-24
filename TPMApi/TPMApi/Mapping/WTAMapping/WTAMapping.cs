using AutoMapper;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.Base;
using WooCommerceNET.WooCommerce.v3;


namespace TPMApi.Mapping.WTAMapping
{
    public class WTAMapping
    {
        private Product _product;

        public List<JObject> MappingData(List<Product> WooProducts)
        {
            var productsAsJObject = new List<JObject>();

            for (var i = 0; i < WooProducts.Count; i++)
            {
                _product = WooProducts[i];

                var products = new AfostoProductPoco()
                {
                    Weight = _product.weight,
                    Cost = 0,
                    Is_Backorder_Allowed = _product.backorders_allowed,
                    Is_Tracking_Inventory = false,
                    Descriptors = SetDescriptors(),
                    Items = SetItems(),
                    Images = null,
                    Specifications = null,
                };

                var ProductAsjsonString = JsonConvert.SerializeObject(products);
                JObject productAsJsonObject = JObject.Parse(ProductAsjsonString);

                productsAsJObject.Add(productAsJsonObject);
            }

            return productsAsJObject;
        }

        private Descriptors SetDescriptors()
        {
            var descriptors = new Descriptors()
            {
                description = _product.description,
                Name = _product.name,
                Seo = SetSeo(),
                MetaGroup = SetMetaGroup(),
                Short_Description = _product.short_description
            };

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

        private MetaGroup SetMetaGroup()
        {
            var metaGroup = new MetaGroup()
            {
                Id = 1390
            };

            return metaGroup;
        }

        private Items SetItems()
        {
            var items = new Items()
            {
                Ean = Convert.ToInt64(DateTime.Now.Ticks.ToString().Substring(0, 13)),
                Sku = "",
                Inventory = SetInventory(),
                Suffix = null,
                Cost = _product.regular_price,
                Options = null,
                Prices = SetPrices(),
                PriceGroup = null
            };

            return items;
        }

        private Inventory SetInventory()
        {
            var inventory = new Inventory()
            {
                Total = 50,
                Warehouses = SetWareHouses()
            };

            return inventory;
        }

        private Warehouses SetWareHouses()
        {
            var warehouses = new Warehouses()
            {
                Id = 1464,
                Amount = 23,
            };

            return warehouses;
        }

        private Options SetOptions()
        {
            var options = new Options()
            {
                Key = "Prijs",
                Value = "75"
            };

            return options;
        }

        private Prices SetPrices()
        {
            var prices = new Prices()
            {
                IsEnabled = true,
                PriceGross = _product.price,
                TaxClass = SetTaxClass()
            };

            return prices;
        }

        private TPMHelper.AfostoHelper.ProductModel.TaxClass SetTaxClass()
        {
            var taxClass = new TPMHelper.AfostoHelper.ProductModel.TaxClass()
            {
                Id = 2
            };

            return taxClass;
        }
    }
}
