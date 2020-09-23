using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Items
    {
        [JsonProperty("ean")]
        public long? Ean { get; set; }
        [JsonProperty("sku")]
        public string Sku { get; set; }
        [JsonProperty("cost")]
        public decimal? Cost { get; set; }
        [JsonProperty("inventory")]
        public Inventory Inventory { get; set; }
        [JsonProperty("options")]
        public Options Options { get; set; }
        [JsonProperty("suffix")]
        public string Suffix { get; set; }
        [JsonProperty("prices")]
        public Prices Prices { get; set; }
    }
}
