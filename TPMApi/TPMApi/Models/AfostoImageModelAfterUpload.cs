using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TPMApi.Models
{
    public class AfostoImageModelAfterUpload
    {
        [JsonProperty("image_id")]
        public int Id { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
