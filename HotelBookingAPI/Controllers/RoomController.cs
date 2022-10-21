using hotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace mongoTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {


        private IMongoCollection<Room> _roomCollection;

        public RoomController(IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _roomCollection = database.GetCollection<Room>("Rooms");
        }

        [HttpGet]
        public IEnumerable<Room> Get()
        {
            return _roomCollection.Find(_ => true).ToList();
        }
    }
}