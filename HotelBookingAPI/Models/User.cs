using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using HotelBookingAPI.Models;

namespace hotelBooking.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonElement("username")]
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string? Password { get; set; }
        [BsonElement("bookedRooms")]
        [JsonPropertyName("bookedRooms")]
        public List<BookedRoomInfo>? BookedRooms { get; set; }
    }
}
