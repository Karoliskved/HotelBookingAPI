using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace hotelBooking.Models
{
    public class GeographicData
    {
        [BsonElement("distToBeach")]
        [JsonPropertyName("distToBeach")]
        public double? DistToBeach { get; set; }
        [BsonElement("distToMountains")]
        [JsonPropertyName("distToMountains")]
        public double? DistTomountains { get; set; }
        [BsonElement("distToForest")]
        [JsonPropertyName("distToForest")]
        public double? DistToForest { get; set; }
        [BsonElement("distToCity")]
        [JsonPropertyName("distToCity")]
        public double? DistToCity { get; set; }
        [BsonElement("distToShop")]
        [JsonPropertyName("distToShop")]
        public double? DistToShop { get; set; }
        [BsonElement("distToRestaurant")]
        [JsonPropertyName("distToRestaurant")]
        public double? DistToRestaurant { get; set; }
        [BsonElement("extraInfo")]
        [JsonPropertyName("extraInfo")]
        public string? ExtraInfo { get; set; }
    }
}
