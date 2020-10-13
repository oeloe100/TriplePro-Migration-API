using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using TPMApi.Helpers;
using TPMHelper.AfostoHelper.Algorithms;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public class SteigerhoutCustomOptionsBuilder : ISteigerhoutCustomOptionsBuilder
    {
        public Product Product;
        public List<JArray> AfostoProductRequirements;
        public JToken TaxClass;
        public List<Items> Items;

        private List<Dictionary<string, string>> _optionsList;

        public SteigerhoutCustomOptionsBuilder(
            Product product,
            List<JArray> afostoProductRequirements,
            JToken taxClass)
        {
            Product = product;
            AfostoProductRequirements = afostoProductRequirements;
            TaxClass = taxClass;
        }

        public void BuildCustomOptions(
            List<Items> items,
            List<Variation> variations)
        {
            Items = items;

            IGAPCG iGapCG = new GAPCGAlgorithm();

            List<List<string>> CombinationsList = new List<List<string>>();

            FillComboList(CombinationsList);

            var result = iGapCG.GAPCG(CombinationsList);

            SetCustomItems(items, result);
        }

        public void FillComboList(List<List<string>> CombinationsList)
        {
            foreach (var attr in Product.attributes)
            {
                if (attr.variation == true &&
                    attr.visible == false &&
                    attr.name.ToLower() != "afwerkingen" &&
                    attr.name.ToLower() != "afwerkingen blad" &&
                    attr.name.ToLower() != "Lange kant boomstam/recht")
                {
                    CombinationsList.Add(attr.options.ToList());
                }
            }

            CombinationsList.Add(SteigerhoutOptionsData.CoationgOptions());
            CombinationsList.Add(SteigerhoutOptionsData.WashingOptions());
        }

        public void SetCustomItems(
            List<Items> itemsList,
            List<List<string>> result)
        {
            for (var i = 0; i < result.Count; i++)
            {
                var option = SortKeyValuePairByOrigin(result[i]);

                var item = new Items()
                {
                    Ean = AfostoProductBuildingHelpers.EanCheck(null),
                    Sku = AfostoProductBuildingHelpers.SKUGenerator(Product,
                                AfostoProductBuildingHelpers.UniqueShortNumberGenerator()),
                    Inventory = SetCustomInventory(0),
                    Prices = SetCustomPrices(0),
                    Options = BuildOptions(option),
                    Suffix = null
                };

                ItemPriceAdjustment(item);
                itemsList.Add(item);
            }
        }

        public List<Dictionary<string, string>> SortKeyValuePairByOrigin(List<string> combination)
        {
            _optionsList = new List<Dictionary<string, string>>();

            foreach (var option in combination)
            {
                if (_optionsList.Count < combination.Count)
                {
                    var coatingChallange = SteigerhoutOptionsData.CoationgOptions().Where(a => a == option);
                    if (IsAny(coatingChallange))
                    {
                        ManagePriceAndOptionsList("coating", option);
                    }

                    var washingChallange = SteigerhoutOptionsData.WashingOptions().Where(a => a == option);
                    if (IsAny(washingChallange))
                    {
                        ManagePriceAndOptionsList("washing", option);
                    }

                    for (var i = 0; i < Product.attributes.Count; i++)
                    {
                        if (Product.attributes[i].variation == true)
                        {
                            var attribute = Product.attributes[i].options.Where(a => a == option);
                            if (IsAny(attribute))
                            {
                                ManagePriceAndOptionsList(
                                    Product.attributes[i].name,
                                    option);
                            }
                        }
                    }
                }
            }

            return _optionsList;
        }

        public void ManagePriceAndOptionsList(
            string type,
            string option)
        {
            var dict = new Dictionary<string, string>
            {
                { type, option }
            };

            _optionsList.Add(dict);
        }

        public bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
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

        public List<Prices> SetCustomPrices(decimal? price)
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

        public List<Options> BuildOptions(List<Dictionary<string, string>> dictList)
        {
            List<Options> options = new List<Options>();

            foreach (var item in dictList)
            {
                foreach (KeyValuePair<string, string> kvp in item)
                {
                    var option = new Options()
                    {
                        Key = kvp.Key,
                        Value = kvp.Value.Split(",")[0]
                    };

                    options.Add(option);
                }
            }

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
    }
}
