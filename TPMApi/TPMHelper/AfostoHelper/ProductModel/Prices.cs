using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Prices
    {
        [JsonProperty("price_Gross")]
        public decimal? PriceGross { get; set; }
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; } = true;
        [JsonProperty("tax_class")]
        public TaxClass TaxClass { get; set; }
    }
}
