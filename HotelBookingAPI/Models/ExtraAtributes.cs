using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class ExtraAtributes
    {

        [BsonElement("wifi")]
        [JsonPropertyName("wifer")]
        public bool? Wifi { get; set; }
        [BsonElement("scenicView")]
        [JsonPropertyName("scenicView")]
        public bool? ScenicView { get; set; }
        [BsonElement("roomService")]
        [JsonPropertyName("roomService")]
        public bool? RoomService { get; set; }
        [BsonElement("tv")]
        [JsonPropertyName("tv")]
        public bool? Tv { get; set; }
        [BsonElement("breakfest")]
        [JsonPropertyName("breakfest")]
        public bool? Breakfest { get; set; }
        [BsonElement("lunch")]
        [JsonPropertyName("lunch")]
        public bool? Lunch { get; set; }
        [BsonElement("diner")]
        [JsonPropertyName("diner")]
        public bool? Diner { get; set; }
    }
}
