using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Helpers
{
    public static class AfostoProductBuildingHelpers
    {
        /// <summary>
        /// Check if sourceEan is null. If so generate custom ean
        /// </summary>
        /// <param name="sourceEan"></param>
        /// <returns></returns>
        public static string EanCheck(string sourceEan)
        {
            if (string.IsNullOrEmpty(sourceEan))
            {
                var ean = Convert.ToInt64(EANGenenerator());
                return ean.ToString();
            }

            return sourceEan;
        }

        /// <summary>
        /// Generate Custom Random EAN Number.
        /// </summary>
        /// <returns></returns>
        public static string EANGenenerator()
        {
            Random generator = new Random();
            String number = generator.Next(0, 1000000).ToString("D6");
            number += generator.Next(0, 10000000).ToString("D7");

            if (number.Distinct().Count() == 1)
            {
                number = EANGenenerator();
            }

            return number;
        }

        public static int? UniqueShortNumberGenerator()
        {
            var numbersArray = Enumerable.Range(0, 10).ToArray();
            var random = new Random();
            int uniqueNumber = 0;

            // Shuffle the array
            for (int i = 0; i < numbersArray.Length; ++i)
            {
                int randomIndex = random.Next(numbersArray.Length);
                int temp = numbersArray[randomIndex];
                numbersArray[randomIndex] = numbersArray[i];
                numbersArray[i] = temp;
            }

            for (var i = 0; i < numbersArray.Length; i++)
            {
                uniqueNumber += numbersArray[i] * Convert.ToInt32(Math.Pow(10, numbersArray.Length - i - 1));
            }

            return uniqueNumber;
        }

        /// <summary>
        /// Generate SKU based on product title + Recource ID.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SKUGenerator(Product product, int? id)
        {
            var stringBuilder = new StringBuilder();
            var words = product.name.Split(new char[] { '-', ' ' });

            for (var i = 0; i < words.Count(); i++)
            {
                stringBuilder.Append(words[i]);
            }

            return stringBuilder + id.ToString();
        }

        public static decimal? PriceGross(decimal? price)
        {
            var vat = (price / 100) * 21;
            var priceGross = (price - vat);

            return priceGross;
        }
    }
}
