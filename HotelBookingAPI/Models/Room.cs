using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hotelBooking.Models
{
    [BsonIgnoreExtraElements]
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("roomNumber")]
        public decimal RoomNumber { get; set; }
        //public string hotelName { get; set; }
        //public decimal capacity { get; set; }
        //public Address address { get; set; }
        //public decimal price { get; set; }
        //public string description { get; set; }
        //public string imageUrlLink { get; set; }
        //public string contactInfo { get; set; }
        //public bool status { get; set; }
    }

}