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

        private List<long> _usedIds;

        private readonly ISortKVP<List<string>> _sortKVP;
        private readonly IPriceCalculator<List<Dictionary<string, string>>> _priceCalculator;

        public SteigerhoutCustomOptionsBuilder(
            Product product,
            List<JArray> afostoProductRequirements,
            JToken taxClass,
            List<long> usedIds)
        {
            Product = product;
            AfostoProductRequirements = afostoProductRequirements;
            TaxClass = taxClass;
            _usedIds = usedIds;

            _sortKVP = new SortKVP(product);
            _priceCalculator = new PriceCalculator(product);
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

            SetCustomItems(items, result, variations);
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
            List<List<string>> result,
            List<Variation> variations)
        {
            for (var i = 0; i < result.Count; i++)
            {
                var option = _sortKVP.SortKeyValuePairByOrigin(result[i]);
                var price = _priceCalculator.Price(option, variations);

                var item = new Items()
                {
                    Ean = AfostoProductBuildingHelpers.EAN13Sum(_usedIds),
                    Sku = AfostoProductBuildingHelpers.SKUGenerator(Product, i),
                    Inventory = SetCustomInventory(0),
                    Options = BuildOptions(option),
                    Prices = SetCustomPrices(price),
                    Suffix = null
                };

                ItemPriceAdjustment(item);
                itemsList.Add(item);
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
