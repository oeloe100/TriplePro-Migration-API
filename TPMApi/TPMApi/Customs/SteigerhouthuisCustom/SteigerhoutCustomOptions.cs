using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using TPMApi.Helpers;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public class SteigerhoutCustomOptions : ISteigerhoutCustomOptions
    {
        public Product Product;
        public List<JArray> AfostoProductRequirements;
        public JToken TaxClass;

        public SteigerhoutCustomOptions(
            Product product,
            List<JArray> afostoProductRequirements,
            JToken taxClass)
        {
            Product = product;
            AfostoProductRequirements = afostoProductRequirements;
            TaxClass = taxClass;
        }

        public void BuildNewWashings(List<Items> items)
        {
            if (Product.attributes.Where(x => x.variation == true
                && x.name.ToLower() != "afwerkingen").Count() < 2)
            {
                foreach (var option in BuildCustomOptions())
                {
                    items.Add(option);
                }
            }
        }

        public List<Items> BuildCustomOptions()
        {
            List<Items> itemsList = new List<Items>();

            var coatingOptions = SteigerhoutOptionsData.NanoCoatingOptions();
            SetCustomItems(coatingOptions, itemsList, false);

            var washingOptions = SteigerhoutOptionsData.WashingsOptions();
            SetCustomItems(washingOptions, itemsList, true);

            return itemsList;
        }

        public void SetCustomItems(
            IDictionary<string, List<string>> options,
            List<Items> itemsList,
            bool isWashing)
        {
            foreach (var coatingItem in options)
            {
                var key = coatingItem.Key;
                foreach (var value in coatingItem.Value)
                {
                    var item = new Items()
                    {
                        Ean = AfostoProductBuildingHelpers.EanCheck(null),
                        Sku = AfostoProductBuildingHelpers.SKUGenerator(Product,
                            AfostoProductBuildingHelpers.UniqueShortNumberGenerator()),
                        Inventory = SetCustomInventory(0),
                        Prices = SetCustomPrices(FinishTypePriceRange(isWashing)),
                        Options = BuildOptions(key, value),
                        Suffix = null
                    };

                    ItemPriceAdjustment(item);
                    itemsList.Add(item);
                }
            }
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
