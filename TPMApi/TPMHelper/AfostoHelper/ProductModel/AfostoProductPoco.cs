﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class AfostoProductPoco
    {
        [JsonProperty("weight")]
        public decimal? Weight { get; set; }
        [JsonProperty("cost")]
        public decimal? Cost { get; set; }
        [JsonProperty("is_tracking_inventory")]
        public bool? Is_Tracking_Inventory { get; set; } = true;
        [JsonProperty("is_backorder_allowed")]
        public bool? Is_Backorder_Allowed { get; set; } = false;
        [JsonProperty("descriptors")]
        public List<Descriptors> Descriptors { get; set; }
        [JsonProperty("specification")]
        public List<Specifications> Specifications { get; set; }
        [JsonProperty("items")]
        public List<Items> Items { get; set; }
        [JsonProperty("images")]
        public List<Images> Images { get; set; }
        [JsonProperty("collections")]
        public JArray Collections { get; set; }
        [JsonProperty("settings")]
        public List<Settings> Settings { get; set; }
    }
}