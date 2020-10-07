using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Builder.Afosto.SteigerhouthuisCustom
{
    public class CustomOptions
    {
        public static IDictionary<string, List<string>> NanoCoatingOption()
        {
            IDictionary<string, List<string>> coatings = new Dictionary<string, List<string>>();
            var coatingTypes = new List<string> { "Geen", "Nano" };
            
            coatings.Add("coating", coatingTypes);

            return coatings;
        }

        public static List<IDictionary<string, string>> WashingsList()
        {
            List<IDictionary<string, string>> washingOptions = new List<IDictionary<string, string>>();

            string[] normalWashings = { "Geen", "Blackwash", "Greywash", "Whitewash", "Brownwash" };
            string[] comboWashings = { "Brown/Whitewash", "Grey/Whitewash", "Black/Whitewash" };

            washingOptions.Add(AddWashingType(normalWashings));
            washingOptions.Add(AddWashingType(comboWashings));

            return washingOptions;
        }

        private static Dictionary<string, string> AddWashingType(string[] washingType)
        {
            var dict = new Dictionary<string, string>();
            var key = "washing";

            foreach (var washing in washingType)
            {
                dict.Add(key, washing);
            }

            return dict;
        }
    }
}
