using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class AditionalPurchases
    {

        [BsonElement("wifi")]
        [JsonPropertyName("wifi")]
        public double? Wifi { get; set; }
        [BsonElement("roomService")]
        [JsonPropertyName("roomService")]
        public double? RoomService { get; set; }
        [BsonElement("tv")]
        [JsonPropertyName("tv")]
        public double? Tv { get; set; }
        [BsonElement("breakfest")]
        [JsonPropertyName("breakfest")]
        public double? Breakfest { get; set; }
        [BsonElement("lunch")]
        [JsonPropertyName("lunch")]
        public double? Lunch { get; set; }
        [BsonElement("diner")]
        [JsonPropertyName("diner")]
        public double? Diner { get; set; }
        [BsonElement("poolAccess")]
        [JsonPropertyName("poolAccess")]
        public double? PoolAccess { get; set; }
    }

}
