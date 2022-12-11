using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models
{
    public class CancellationInfo
    {
        [Required]
        [BsonElement("userID")]
        [JsonPropertyName("userID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserID { get; set; }
        [Required]
        [BsonElement("roomID")]
        [JsonPropertyName("roomID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RoomID { get; set; }
    }
}
