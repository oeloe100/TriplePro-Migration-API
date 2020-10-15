using System.Collections.Generic;
using System.Linq;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public class PriceCalculator : IPriceCalculator<List<Dictionary<string, string>>>
    {
        private Product _product;

        public PriceCalculator(Product product)
        {
            _product = product;
        }

        public decimal? Price(
            List<Dictionary<string, string>> optionList, 
            List<Variation> variations)
        {

            var priceList = new List<decimal>()
            {
                (decimal)GetBasePrice(optionList, variations),
                GetCoatingPrice(optionList),
                GetWashingPrice(optionList)
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
            foreach (var dict in optionList)
            {
                foreach (var value in dict.Values)
                {
                    var basePrice = SetPrice(variations, value);
                    if (basePrice.HasValue)
                    {
                        return basePrice;
                    }

                    return 0;
                }
            }

            return null;
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

        private decimal GetWashingPrice(List<Dictionary<string, string>> optionList)
        {
            foreach (var dict in optionList)
            {
                foreach (var value in dict.Values)
                {
                    var test = SteigerhoutOptionsData.WashingOptions().Where(x => x == value);
                    if (IsAny(test) && 
                        value.ToLower().Equals("geen") == false)
                    {
                        if (IsSpecialCategory("Loungebanken") ||
                            IsSpecialCategory("Kasten"))
                            return 40;

                        return 20;
                    }
                }
            }

            return 0;
        }

        private decimal GetCoatingPrice(List<Dictionary<string, string>> optionList)
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
        }

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
    }
}
