﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class RoomBookingInfo
    {
        [BsonElement("userID")]
        [JsonPropertyName("userID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserID { get; set; }
        [BsonElement("roomID")]
        [JsonPropertyName("roomID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RoomID { get; set; }
        [BsonElement("from")]
        [JsonPropertyName("from")]
        public DateTime FromDate { get; set; }
        [BsonElement("to")]
        [JsonPropertyName("to")]
        public DateTime ToDate { get; set; }
    }
}
