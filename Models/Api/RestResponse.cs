using System.Text.Json.Serialization;

namespace AspKnP231.Models.Api
{
    public class RestResponse
    {
        public RestMeta Meta { get; set; } = new();
        public Object? Data { get; set; }
    }

    public class RestMeta
    {
        [JsonPropertyName("serverTime")]
        public long ServerTime { get; set; }

        [JsonPropertyName("totalCount")]
        public int? TotalCount { get; set; }

        [JsonPropertyName("cache")]
        public long Cache { get; set; }

        [JsonPropertyName("dataType")]
        public String DataType { get; set; } = "null";

        [JsonPropertyName("resourceId")]
        public String? ResourceId { get; set; }

        [JsonPropertyName("authStatus")]
        public String AuthStatus { get; set; } = "UnAuthorized";

        [JsonPropertyName("path")]
        public String Path { get; set; } = "/";

        [JsonPropertyName("service")]
        public String Service { get; set; } = "Asp-Shop API";
    }
}