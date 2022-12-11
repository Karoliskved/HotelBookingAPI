using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class BookedRoomInfo : RoomBookingInfo
    {
        [BsonElement("price")]
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
    }
}
