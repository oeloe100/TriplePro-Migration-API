using System;
using System.Collections.Generic;
using System.Linq;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public class PriceCalculator : IPriceCalculator<List<Dictionary<string, string>>>
    {
        private Product _product;
        private int hitCount = 0;

        public PriceCalculator(Product product)
        {
            _product = product;
        }

        public decimal? Price(
            List<Dictionary<string, string>> optionList, 
            List<Variation> variations,
            IDictionary<string, bool> bundledAccessManger)
        {
            Filter(optionList);

            var priceList = new List<decimal>()
            {
                (decimal)GetBasePrice(optionList, variations),
                //GetCoatingPrice(optionList),
                GetWashingPrice(optionList, bundledAccessManger)
            };

            var totalPrice = CalculatePrice(priceList);
            return totalPrice;
        }

        private decimal CalculatePrice(
            List<decimal> priceList, 
            decimal total = 0)
        {
            for (var i = 0; i < priceList.Count; i++)
            {
                total += priceList[i];
            }

            return total;
        }

        private decimal? GetBasePrice(
            List<Dictionary<string, string>> optionList, 
            List<Variation> variations)
        {
            if (variations.Count > 1)
            {
                for (var x = 0; x < variations.Count; x++)
                {
                    if (variations[x].attributes.Count == optionList.Count)
                    {
                        var price = GetPrice(optionList, variations, x);
                        if (price != -1)
                            return price;
                    }
                    else
                    {
                        foreach (var dict in optionList)
                        {
                            foreach (var value in dict.Values)
                            {
                                var basePrice = SetPrice(variations, value);
                                if (basePrice.HasValue)
                                {
                                    return basePrice;
                                }
                            }
                        }
                    }
                }
            }

            if (variations.Count <= 1)
            {
                foreach (var variation in variations)
                {
                    return variation.price;
                }
            }

            return 0;
        }

        private decimal GetPrice(
            List<Dictionary<string, string>> optionList,
            List<Variation> variations,
            int x)
        {
            hitCount = 0;

            for (var i = 0; i < variations[x].attributes.Count; i++)
            {
                var value = GetValue(optionList, variations[x], i);
                if (!value.Equals(default(KeyValuePair<string, string>)))
                {
                    hitCount++;
                    if (hitCount == optionList.Count)
                    {
                        var price = variations[x].price;
                        return (decimal)price;
                    }

                    continue;
                }
            }

            return -1;
        }

        private KeyValuePair<string, string> GetValue(
            List<Dictionary<string, string>> optionList,
            Variation variation,
            int i)
        {
            var value = optionList[i].FirstOrDefault(x => x.Value == variation.attributes[i].option);
            return value;
        }

        private decimal? SetPrice(
            List<Variation> variations,
            string value)
        {
            foreach (var variation in variations)
            {
                foreach (var attribute in variation.attributes)
                {
                    if (attribute.option.Equals(value))
                    {
                        return variation.price;
                    }
                }
            }

            return null;
        }

        /*
        private decimal? SetPrice(
            List<Variation> variations, 
            string value)
        {
            foreach (var variation in variations)
            {
                foreach (var attribute in variation.attributes)
                {
                    if (attribute.option.Equals(value))
                    {
                        return variation.price;
                    }
                }
            }

            return null;
        }
        */

        private decimal GetWashingPrice(
        List<Dictionary<string, string>> optionList,
        IDictionary<string, bool> bundledAccessManger)
        {
            if (bundledAccessManger["isBundle"] && 
                !bundledAccessManger["isParent"])
            {
                if (IsSpecialCategory("Loungebanken") ||
                    IsSpecialCategory("Kasten"))
                    return 40;

                return 20;
            }

            return 0;
        }

        /*private decimal GetCoatingPrice(List<Dictionary<string, string>> optionList)
        {
            foreach (var dict in optionList)
            {
                foreach (var value in dict.Values)
                {
                    var test = SteigerhoutOptionsData.CoationgOptions().Where(x => x == value);
                    if (IsAny(test) && 
                        value.ToLower().Equals("geen") == false)
                    {
                        if (IsSpecialCategory("Loungebanken"))
                            return 50;

                        return 30;
                    }
                }
            }

            return 0;
        }*/

        private bool IsSpecialCategory(string categoryName)
        {
            for (var i = 0; i < _product.categories.Count(); i++)
            {
                if (_product.categories[i].name == categoryName)
                    return true;
            }

            return false;
        }

        private bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        private void Filter(List<Dictionary<string, string>> optionList)
        {
            foreach (var option in optionList)
            {
                foreach (var key in option.Keys)
                { 
                    if (key.ToLower() == "afwerkingen blad/zitting")
                    {
                        optionList.Remove(option);
                    }
                }
            }
        }
    }
}
