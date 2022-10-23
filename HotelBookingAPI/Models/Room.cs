using HotelBookingAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace hotelBooking.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }
        [BsonElement("hotel")]
        [JsonPropertyName("hotel")]
        public Hotel? Hotel { get; set; }
        [BsonElement("roomNumber")]
        [JsonPropertyName("roomNumber")]
        public decimal RoomNumber { get; set; }
        [BsonElement("capacity")]
        [JsonPropertyName("capacity")]
        public decimal Capacity { get; set; }
        [BsonElement("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [BsonElement("imageUrlLink")]
        [JsonPropertyName("imgUrlLink")]
        public string? ImageUrlLink { get; set; }

        [BsonElement("booked")]
        [JsonPropertyName("booked")]
        public bool Booked { get; set; }
    }

}