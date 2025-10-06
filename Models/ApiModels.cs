using System.Text.Json.Serialization;

namespace ABCRetail.Models
{
    public class ApiModels
    {
        //Attributes map the JSON property
        //C# property
        [JsonPropertyName("name")]

        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

    }
}
