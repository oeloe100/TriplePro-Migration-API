using System.Collections.Generic;

namespace TPMApi.Helpers
{
    public class BundledProductAccessHelper
    {
        public static Dictionary<string, bool> SetAccess(bool isBundle, bool isParent)
        {
            var accessDictionary = new Dictionary<string, bool>();

            accessDictionary.Add("isBundle", isBundle);
            accessDictionary.Add("isParent", isParent);

            return accessDictionary;
        }
    }
}
