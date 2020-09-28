using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Descriptors
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("short_description")]
        public string Short_Description { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("seo")]
        public Seo Seo { get; set; }
        [JsonProperty("meta_group")]
        public JToken MetaGroup { get; set; }
    }
}
