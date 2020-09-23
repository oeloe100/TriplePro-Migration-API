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


namespace TPMApi.Mapping.WooCommerceToAfostoMapping
{
    public class WTAMapping
    {
        private Product _product;

        public JObject MappingData(List<Product> WooProducts)
        {
            for (var i = 0; i < WooProducts.Count; i++)
            {
                _product = WooProducts[i];

                var products = new AfostoProductPoco()
                {
                    Weight = 12000,
                    Cost = 0,
                    Is_Backorder_Allowed = _product.backorders_allowed,
                    Is_Tracking_Inventory = false,
                    Descriptors = SetDescriptors(),
                    Items = SetItems(),
                    Specifications = Specifications()
                };

                var newTest = JsonConvert.SerializeObject(products);
                JObject test = JObject.Parse(newTest);

                return test;
            }

            return null;
        }

        private Specifications Specifications()
        {
            var spec = new Specifications()
            {
                Key = _product.attributes[0].options[0],
                Value = _product.attributes[0].name
            };

            return spec;
        }

        private Descriptors SetDescriptors()
        {
            var descriptors = new Descriptors()
            {
                description = _product.description,
                Name = _product.name,
                Seo = SetSeo(),
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

        private Items SetItems()
        {
            var items = new Items()
            {
                Ean = Convert.ToInt64(DateTime.Now.Ticks.ToString().Substring(0, 13)),
                Sku = "",
                Inventory = SetInventory(),
                Suffix = "",
                Cost = _product.regular_price,
                Options = SetOptions(),
                Prices = SetPrices()
            };

            return items;
        }

        private Inventory SetInventory()
        {
            var inventory = new Inventory()
            {
                Total = 50
            };

            return inventory;
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
