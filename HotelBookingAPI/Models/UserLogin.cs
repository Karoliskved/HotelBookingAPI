using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class UserLogin
    {
        [Required]
        [BsonElement("username")]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [Required]
        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
