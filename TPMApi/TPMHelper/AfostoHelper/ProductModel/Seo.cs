using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Seo
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("keywords")]
        public string Keywords { get; set; }
        [JsonProperty("robots")]
        public string robots { get; set; } = null;
    }
}
