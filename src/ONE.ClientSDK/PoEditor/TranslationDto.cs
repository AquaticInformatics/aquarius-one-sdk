using Newtonsoft.Json;

namespace ONE.ClientSDK.PoEditor
{
    [JsonObject(Title = "translation")]
    public class TranslationDto
    {
        /// <summary>
        /// Corresponds to the I18NKey.
        /// </summary>
        [JsonProperty(PropertyName = "term")]
        public string Term { get; set; }

        /// <summary>
        /// Corresponds to the translation.
        /// </summary>
        [JsonProperty(PropertyName = "definition")]
        public string Definition { get; set; }

        [JsonProperty(PropertyName = "context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "term_plural")]
        public string TermPlural { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
    }
}
