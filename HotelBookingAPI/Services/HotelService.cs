using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace HotelBookingAPI.Services
{
    public class HotelService : IHotel
    {
        private readonly IMongoCollection<Hotel> _hotelCollection;

        public HotelService(IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _hotelCollection = database.GetCollection<Hotel>("hotels");
        }

        public async Task<List<Hotel>> GetAllHotels()
        {
            return await _hotelCollection.Find(_ => true).ToListAsync();
        }
        public async Task<Hotel> GetHotelById(string? id)
        {
            return await _hotelCollection.Find(hotel => hotel.ID == id).FirstOrDefaultAsync();
        }
        public async Task<Hotel> GetHotelByDistToBeach(double? id)
        {
            return await _hotelCollection.Find(hotel => hotel.GeographicData.DistToBeach <= id).FirstOrDefaultAsync();
        }
        public async Task<List<Hotel>> GetHotelByMultiParam(string[] atributes, double[] distances)
        {
            var _tempHotelcollection = _hotelCollection;
            var builder = Builders<Hotel>.Filter;
            var filter = builder.Lt(atributes[0], distances[0]);
            for(int i = 1; i < atributes.Length; i++)
            {
                filter=filter & builder.Lt(atributes[i], distances[i]); 
               // _tempHotelcollection= _tempHotelcollection.Find(hotel =>  hotel.GeographicData.GetType().GetProperty("DistToShop").GetValue(hotel.GeographicData, null) == distances[i]).FirstOrDefaultAsync();
            }
            return await _hotelCollection.Find(filter).ToListAsync();
        }
        public async Task<List<Hotel>> GetHotelByClosestTotheBeach()
        {

            return await _hotelCollection.Find(_ => true).Limit(10).Sort(Builders<Hotel>.Sort.Ascending("GeographicData.DistToBeach")).ToListAsync();
        }
    }
    
}
