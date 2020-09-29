using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
