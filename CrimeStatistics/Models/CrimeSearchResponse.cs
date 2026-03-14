using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrimeStatistics.Models
{
    public class CrimeSearchResponse
    {
        [JsonPropertyName("search_radius_metres")]
        public int SearchRadiusMetres { get; set; } = 1609;

        [JsonPropertyName("search_center_lat")]
        public double SearchCenterLat { get; set; }

        [JsonPropertyName("search_center_lng")]
        public double SearchCenterLng { get; set; }

        [JsonPropertyName("crimes")]
        public JsonElement Crimes { get; set; }
    }
}
