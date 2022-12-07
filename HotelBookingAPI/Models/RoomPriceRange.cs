
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class RoomPriceRange
    {
        [BsonElement("price")]
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        [BsonElement("from")]
        [JsonPropertyName("from")]
        public DateTime FromDate { get; set; }
        [BsonElement("to")]
        [JsonPropertyName("to")]
        public DateTime ToDate { get; set; }
    }
}
