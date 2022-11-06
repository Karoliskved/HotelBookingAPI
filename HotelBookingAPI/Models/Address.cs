using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace hotelBooking.Models
{
    public class Address
    {
        [BsonElement("city")]
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [BsonElement("country")]
        [JsonPropertyName("country")]
        public string? Country { get; set; }
        [BsonElement("street")]
        [JsonPropertyName("street")]
        public string? Street { get; set; }
        [BsonElement("number")]
        [JsonPropertyName("number")]
        public string? HouseNumber { get; set; }
        [BsonElement("zipCode")]
        [JsonPropertyName("zipCode")]
        public string? ZIPCode { get; set; }
    }
}
