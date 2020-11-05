using Newtonsoft.Json;
using System.Collections.Generic;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Items
    {
        [JsonProperty("ean")]
        public string Ean { get; set; }
        [JsonProperty("sku")]
        public string Sku { get; set; }
        [JsonProperty("cost")]
        public decimal? Cost { get; set; }
        [JsonProperty("inventory")]
        public Inventory Inventory { get; set; }
        [JsonProperty("prices")]
        public List<Prices> Prices { get; set; }
        [JsonProperty("options")]
        public List<Options> Options { get; set; }
    }
}
