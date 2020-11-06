using Newtonsoft.Json;
using System.Collections.Generic;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Collections
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("_links")]
        public List<Links> Links { get; set; }
    }
}
