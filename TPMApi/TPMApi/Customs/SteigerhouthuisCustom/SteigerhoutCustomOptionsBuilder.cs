using Newtonsoft.Json.Linq;
using System;
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

            foreach (var combination in result)
            {
                SetCustomItems(items, combination);
            }
        }

        public void FillComboList(List<List<string>> CombinationsList)
        {
            foreach (var attr in Product.attributes)
            {
                if (attr.variation == true)
                {
                    CombinationsList.Add(attr.options.ToList());
                }
            }

            CombinationsList.Add(SteigerhoutOptionsData.CoationgOptions());
            CombinationsList.Add(SteigerhoutOptionsData.WashingOptions());
        }

        public void SetCustomItems(
            List<Items> itemsList,
            List<string> combinations)
        {
            var option = SortKeyValuePairByOrigin(combinations);

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

            Console.WriteLine();
        }

        public List<Dictionary<string, string>> SortKeyValuePairByOrigin(List<string> combination)
        {
            List<Dictionary<string, string>> optionsList = new List<Dictionary<string, string>>();

            foreach (var option in combination)
            {
                if (optionsList.Count < combination.Count)
                { 
                    var coatingChallange = SteigerhoutOptionsData.CoationgOptions().Where(a => a == option);
                    if (IsAny(coatingChallange))
                    {
                        var dict = new Dictionary<string, string>();
                        dict.Add("coating", option);
                        optionsList.Add(dict);
                    }

                    var washingChallange = SteigerhoutOptionsData.WashingOptions().Where(a => a == option);
                    if (IsAny(washingChallange))
                    {
                        var dict = new Dictionary<string, string>();
                        dict.Add("washing", option);
                        optionsList.Add(dict);
                    }

                    for (var i = 0; i < Product.attributes.Count; i++)
                    {
                        if (Product.attributes[i].variation == true)
                        {
                            var attribute = Product.attributes[i].options.Where(a => a == option);
                            if (IsAny(attribute))
                            {
                                var dict = new Dictionary<string, string>();
                                dict.Add(Product.attributes[i].name, option);
                                optionsList.Add(dict);
                            }
                        }
                    }
                }
            }

            return optionsList;
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
