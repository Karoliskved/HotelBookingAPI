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
        /*[BsonElement("hotel")]
        [JsonPropertyName("hotel")]
        public Hotel? Hotel { get; set; }*/ // dont know if its worth to keep the hotel object here
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
        public List<BookedRoomInfo> BookedDates { get; set; } = new List<BookedRoomInfo>(); //is this not supposed to be RoomBookingInfo?
        [BsonElement("extraAtributes")]
        [JsonPropertyName("extraAtributes")]
        public ExtraAtributes? ExtraAtributes { get; set; }
        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [BsonElement("imageUrlLink")]
        [JsonPropertyName("imageUrlLink")]
        public string? ImageUrlLink { get; set; }
    }

}