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
        [BsonElement("houseNumber")]
        [JsonPropertyName("houseNumber")]
        public string? HouseNumber { get; set; }
    }
}
