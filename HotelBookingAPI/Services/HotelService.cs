using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections;

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
            return await _hotelCollection.Find(hotel => hotel.HotelID == id).FirstOrDefaultAsync();
        }
        public async Task<string?> AddHotel(Hotel hotel)
        {
            await _hotelCollection.InsertOneAsync(hotel);
            return null;
        }
        public async Task<Hotel> GetHotelByDistToBeach(double? id)
        {
            return await _hotelCollection.Find(hotel => hotel.GeographicData.DistToBeach <= id).FirstOrDefaultAsync();
        }
        public async Task<List<Hotel>> GetHotelByMultiParam(string[] atributes, Object[] distances, string[] operators)
        {
            var builder = Builders<Hotel>.Filter;
            var filter = builder.Empty;
            for (int i = 0; i < atributes.Length; i++)
            {
                switch (operators[i])
                {
                    case "Lt":
                        filter = filter & builder.Lt(atributes[i], distances[i]);
                        break;
                    case "Gt":
                        filter = filter & builder.Gt(atributes[i], distances[i]);
                        break;
                    case "Eq":
                        filter = filter & builder.Eq(atributes[i], distances[i]);
                        break;
                }



                // _tempHotelcollection= _tempHotelcollection.Find(hotel =>  hotel.GeographicData.GetType().GetProperty("DistToShop").GetValue(hotel.GeographicData, null) == distances[i]).FirstOrDefaultAsync();
            }
            return await _hotelCollection.Find(filter).ToListAsync();
        }
        public async Task<List<Hotel>> SortHotels(string[] atributes, string[] operators)
        { 
            string sortString = "{";
            var bldr = Builders<Hotel>.Sort;
            int i = 0;
            var sortDefinitions = atributes.Select(x =>
            {
                
                SortDefinition<Hotel> sortDef;
                if (operators[i] == "1")
                {
                    sortDef = bldr.Descending(x);
                }
                else if (operators[i] == "0")
                {
                    sortDef = bldr.Ascending(x);
                }
                else
                {
                    sortDef = null;
                }
                i++;
                return sortDef;
            });
            /* for (int i = 0; i < atributes.Length; i++)
             {
                 sortString += atributes[i] + ": " + operators[i];
                 if (i != atributes.Length - 1)
                 {
                     sortString += ", ";
                 }
             }
             sortString += "}";*/
            var sortDef = bldr.Combine(sortDefinitions);
           
            return await _hotelCollection.Find(_ => true).Limit(100).Sort(sortDef).ToListAsync();


        }
    }
}
