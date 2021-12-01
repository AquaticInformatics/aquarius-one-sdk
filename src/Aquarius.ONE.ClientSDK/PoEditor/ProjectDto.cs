using System;
using Newtonsoft.Json;

namespace ONE.PoEditor
{
    [JsonObject(Title = "project")]
    public class ProjectDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "public")]
        public bool Public { get; set; }

        [JsonProperty(PropertyName = "open")]
        public bool Open { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }
    }
}
