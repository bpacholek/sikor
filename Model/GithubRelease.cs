using Newtonsoft.Json;

namespace Sikor.Model
{
    public class GithubRelease
    {
        [JsonProperty(PropertyName = "html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty(PropertyName = "tag_name")]
        public string TagName { get; set; }

    }
}