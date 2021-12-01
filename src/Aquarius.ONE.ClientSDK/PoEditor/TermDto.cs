using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ONE.PoEditor
{
    [JsonObject(Title = "term")]
    public class TermDto : IComparable
    {
        /// <summary>
        /// The key that identifies a unique translation in POEditor.
        /// </summary>
        [JsonProperty(PropertyName = "term")]
        public string Term { get; set; }

        [JsonProperty(PropertyName = "context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "plural")]
        public string Plural { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }

        [JsonProperty(PropertyName = "translation")]
        public TranslationDto Translation { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public IList<string> Tags { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonIgnore]
        public string ResourceId => $@"${Context.Replace('.', '$').Replace("\"", string.Empty)}${Term}";

        public int CompareTo(object obj)
        {
            var compareToObj = (obj as TermDto);
            return string.CompareOrdinal(
                string.Join(string.Empty, ResourceId, Reference),
                string.Join(string.Empty, compareToObj?.ResourceId, compareToObj?.Reference)
                );
        }
    }

    /// <summary>
    /// Compares terms for equality based on the ResourceId property.
    /// Does not test for complete equality.
    /// </summary>
    public class TermResourceIdComparer : IEqualityComparer<TermDto>
    {
        /// <inheritdoc />>
        public bool Equals(TermDto x, TermDto y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            return x.ResourceId == y.ResourceId;
        }

        /// <inheritdoc />>
        public int GetHashCode(TermDto obj)
        {
            return obj.ResourceId == null ? 0 : obj.ResourceId.GetHashCode();
        }
    }
}
