using System;
using Newtonsoft.Json;

namespace ONE.ClientSDK.PoEditor
{
    [JsonObject(Title = "language")]
    public class LanguageDto
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "translations")]
        public int? Translations { get; set; }

        [JsonProperty(PropertyName = "percentage")]
        public float? Percentage { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }
    }
}
