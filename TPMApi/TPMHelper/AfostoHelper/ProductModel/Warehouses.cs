using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Warehouses
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
        [JsonProperty("amount")]
        public int? Amount { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; } = null;
    }
}
