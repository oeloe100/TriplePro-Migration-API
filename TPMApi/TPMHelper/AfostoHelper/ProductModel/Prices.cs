using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Prices
    {
        [JsonProperty("price_gross")]
        public decimal? PriceGross { get; set; }
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; } = true;
        [JsonProperty("price")]
        public decimal? Price { get; set; }
        [JsonProperty("original_price_gross")]
        public decimal? OriginalPriceGross { get; set; }
        [JsonProperty("original_price")]
        public decimal? OriginalPrice { get; set; }
        [JsonProperty("price_group")]
        public JToken Price_Group { get; set; }
        [JsonProperty("tax_class")]
        public JToken TaxClass { get; set; }
        [JsonProperty("_links")]
        public JToken Links { get; set; }
    }
}
