using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using HotelBookingAPI.Models;

namespace hotelBooking.Models
{
    public class User
    {
        [BsonId]
        [BsonElement("userID")]
        [JsonPropertyName("userID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserID { get; set; }
        [BsonElement("username")]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        [BsonElement("role")]
        [JsonPropertyName("role")]
        public Role Role { get; set; }
        [BsonElement("bookedRooms")]
        [JsonPropertyName("bookedRooms")]
        public List<BookedRoomInfo> BookedRooms { get; set; } = new List<BookedRoomInfo>();
    }
    public enum Role
    {
        User,
        Administrator
    }
}
