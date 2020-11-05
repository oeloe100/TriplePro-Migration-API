using System.Collections.Generic;

namespace TPMApi.Customs.SteigerhouthuisCustom
{
    public class SteigerhoutOptionsData
    {
        /*public static List<string> CoationgOptions()
        {
            List<string> nanoCoatingOptions = new List<string>()
            {
                "Geen",
                "Nano"
            };

            return nanoCoatingOptions;
        }*/

        public static List<string> WashingOptions()
        {
            List<string> washingOptions = new List<string>()
            {
                "Geen",
                "Blackwash",
                "Whitewash",
                "Brownwash",
            };

            return washingOptions;
        }
    }
}
