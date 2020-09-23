using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Images
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("is_default")]
        public bool IsDefault { get; set; } = false;
    }
}
