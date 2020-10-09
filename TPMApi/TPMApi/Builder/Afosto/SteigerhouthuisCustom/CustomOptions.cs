using System;
using System.Collections.Generic;
using System.Linq;

namespace TPMApi.Builder.Afosto.SteigerhouthuisCustom
{
    public class CustomOptions
    {
        public static IDictionary<string, List<string>> NanoCoatingOptions()
        {
            IDictionary<string, List<string>> coatings = new Dictionary<string, List<string>>();
            var coatingTypes = new List<string> { "Geen", "Nano" };

            try
            {
                coatings.Add("coating", coatingTypes);
                return coatings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static IDictionary<string, List<string>> WashingsOptions()
        {
            IDictionary<string, List<string>> washings = new Dictionary<string, List<string>>();

            string[] normalWashings = { "Geen", "Blackwash", "Greywash", "Whitewash", "Brownwash" };
            string[] comboWashings = { "Brown/Whitewash", "Grey/Whitewash", "Black/Whitewash" };

            AddWashingType(washings, normalWashings);
            AddWashingType(washings, comboWashings);

            return washings;
        }

        private static IDictionary<string, List<string>> AddWashingType(
            IDictionary<string, List<string>> dict,
            string[] washingType)
        {
            var key = "washing";

            try
            {
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, washingType.ToList());
                }
                else
                {
                    foreach (var washing in washingType)
                    {
                        dict[key].Add(washing);
                    }
                }

                return dict;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
