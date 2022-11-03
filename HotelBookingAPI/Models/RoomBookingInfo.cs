using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HotelBookingAPI.Models
{
    public class RoomBookingInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RoomID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
