using Newtonsoft.Json;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Links
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
