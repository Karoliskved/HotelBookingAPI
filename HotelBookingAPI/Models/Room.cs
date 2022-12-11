using HotelBookingAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace hotelBooking.Models
{
    public class Room
    {
        [BsonId]
        [BsonElement("roomID")]
        [JsonPropertyName("roomID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RoomID { get; set; }
        [BsonElement("hotelID")]
        [JsonPropertyName("hotelID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? HotelID { get; set; }
        [BsonElement("roomNumber")]
        [JsonPropertyName("roomNumber")]
        public decimal RoomNumber { get; set; }
        [BsonElement("capacity")]
        [JsonPropertyName("capacity")]
        public decimal Capacity { get; set; }
        [BsonElement("address")]
        [JsonPropertyName("address")]
        public Address? Address { get; set; }
        [BsonElement("priceRanges")]
        [JsonPropertyName("priceRanges")]
        public List<RoomPriceRange> PriceRanges { get; set; } = new List<RoomPriceRange>();
        [BsonElement("bookedDates")]
        [JsonPropertyName("bookedDates")]
        public List<BookedRoomInfo> BookedDates { get; set; } = new List<BookedRoomInfo>();
        [BsonElement("extraAtributes")]
        [JsonPropertyName("extraAtributes")]
        public ExtraAtributes? ExtraAtributes { get; set; }
        [BsonElement("aditionalPurchases")]
        [JsonPropertyName("aditionalPurchases")]
        public AditionalPurchases? AditionalPurchases { get; set; }
        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [BsonElement("imageUrlLink")]
        [JsonPropertyName("imageUrlLink")]
        public string? ImageUrlLink { get; set; }
    }

}