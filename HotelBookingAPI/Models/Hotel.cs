using hotelBooking.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class Hotel
    {
        [BsonId]
        [BsonElement("hotelID")]
        [JsonPropertyName("hotelID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? HotelID { get; set; }
        [BsonElement("hotelName")]
        [JsonPropertyName("hotelName")]
        public string? HotelName { get; set; }
        [BsonElement("address")]
        [JsonPropertyName("address")]
        public Address? Address { get; set; }
        [BsonElement("imageUrlLink")]
        [JsonPropertyName("imageUrlLink")]
        public string? ImageUrlLink { get; set; }
        [BsonElement("geographicData")]
        [JsonPropertyName("geographicData")]
        public GeographicData? GeographicData { get; set; }
        [BsonElement("stars")]
        [JsonPropertyName("stars")]
        public int? Stars { get; set; }
        [BsonElement("contactInfo")]
        [JsonPropertyName("contactInfo")]
        public string? ContactInfo { get; set; }

    }
}
