using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class TaxClass
    {
        [JsonProperty("id")]
        public int? Id { get; set; } = 2;
        [JsonProperty("_links")]
        public JArray Links { get; set; }
    }
}
