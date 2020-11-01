using Newtonsoft.Json.Linq;

namespace TPMApi.Helpers
{
    public class DebugFailedMigrations
    {
        public static bool ByTitle(JArray afostoProducts, string wooTitle)
        {
            for (var y = 0; y < afostoProducts.Count; y++)
            {
                var descriptors = AfostoProductBuildingHelpers.GetValue<JToken>(afostoProducts[y], "descriptors");
                var name = descriptors[0].GetValue<JToken>("name").ToString();

                if (name == wooTitle)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
