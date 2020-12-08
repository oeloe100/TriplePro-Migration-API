using Newtonsoft.Json;

namespace TPMHelper.AfostoHelper.PatchModels
{
    public class PatchModel
    {
        [JsonProperty("op")]
        public string Operation { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
