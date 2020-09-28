using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Inventory
    {
        [JsonProperty("total")]
        public int? Total { get; set; } = 50;
        [JsonProperty("warehouses")]
        public JArray Warehouses { get; set; }
    }
}
