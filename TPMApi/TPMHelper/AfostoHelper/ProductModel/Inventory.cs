using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Inventory
    {
        [JsonProperty("total")]
        public int? Total { get; set; } = 50;
    }
}
