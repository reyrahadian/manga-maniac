using Newtonsoft.Json;

namespace MangaManiac.Core.Models
{
    public class MangaArtifact
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("chapters")]
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }

        public class Chapter
        {
            [JsonProperty("number")]
            public float Number { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("uri")]
            public Uri Uri { get; set; }
        }
    }
}
