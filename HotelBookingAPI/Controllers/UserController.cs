using hotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace mongoTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {


        private IMongoCollection<User> _userCollection;

        public UserController(IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _userCollection = database.GetCollection<User>("Users");
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userCollection.Find(_ => true).ToList();
        }
    }
}