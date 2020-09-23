using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class AfostoProductPoco
    {
        [JsonProperty("weight")]
        public double Weight { get; set; }
        [JsonProperty("cost")]
        public decimal Cost { get; set; }
        [JsonProperty("is_tracking_inventory")]
        public bool? Is_Tracking_Inventory { get; set; } = true;
        [JsonProperty("is_backorder_allowed")]
        public bool? Is_Backorder_Allowed { get; set; } = false;
        [JsonProperty("descriptors")]
        public Descriptors Descriptors { get; set; }
        [JsonProperty("specification")]
        public Specifications Specifications { get; set; }
        [JsonProperty("items")]
        public Items Items { get; set; }
        [JsonProperty("images")]
        public Images Images { get; set; }
    }
}
