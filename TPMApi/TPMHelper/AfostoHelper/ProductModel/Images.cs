using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Images
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("is_default")]
        public bool IsDefault { get; set; } = false;
    }
}
