using hotelBooking.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IMongoCollection<User> _userCollection;
        private IMongoCollection<Room> _roomCollection;
        private IHttpContextAccessor _context;

        public UserController(IMongoClient client, IHttpContextAccessor httpContextAccessor)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _userCollection = database.GetCollection<User>("Users");
            _roomCollection = database.GetCollection<Room>("Room");
            _context = httpContextAccessor;
        }
        // sita route tik adminam reiks padaryt [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ("Administrator")]
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers() =>  await _userCollection.Find(_ => true).ToListAsync();
       
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserByID(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
                
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var user = await _userCollection.Find(user => user.ID == id).FirstOrDefaultAsync();
                if (user.Username != username)
                {
                    return Unauthorized();
                }
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound($"User with id: {id} doesn't exist.");
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("/bookHotelRoom/{id}")]
        public async Task<IActionResult> BookHotelRoomByID(string id,[FromBody] User user)
        {
            if (ObjectId.TryParse(id, out _) || ObjectId.TryParse(user.ID, out _))
            {
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var result = await _userCollection.Find(user => user.ID == id).FirstOrDefaultAsync();
                if (result.Username != username)
                {
                    return Unauthorized();
                }
                var room = await _roomCollection.Find(id).FirstOrDefaultAsync();
            if(room != null)
            {
                FilterDefinition<Room> filter2 = Builders<Room>.Filter.Eq("ID", id);
                UpdateDefinition<Room> update2 = Builders<Room>.Update.Set("Booked", true);
                await _roomCollection.UpdateOneAsync(filter2, update2);
                room = await _roomCollection.Find(id).FirstOrDefaultAsync();
                FilterDefinition<User> filter = Builders<User>.Filter.Eq("ID", user.ID);
                UpdateDefinition<User> update = Builders<User>.Update.AddToSet("BookedRooms", room);
                await _userCollection.UpdateOneAsync(filter, update);
                return CreatedAtAction(nameof(BookHotelRoomByID), new { id = user.ID }, user);
            }
            return NotFound($"Room with id: {id} doesn't exist.");
               
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (ObjectId.TryParse(id, out _))
            {
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var result = await _userCollection.Find(user => user.ID == id).FirstOrDefaultAsync();
                if (result.Username != username)
                {
                    return Unauthorized();
                }
                var room = await _userCollection.Find(user => user.ID == id).FirstOrDefaultAsync();
                if (user is null)
                {
                    return NotFound();
                }
                await _userCollection.ReplaceOneAsync(user => user.ID == id, user);
                return NoContent();
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var user = await _userCollection.Find(user => user.ID == id).FirstOrDefaultAsync();
                if (user.Username != username)
                {
                    return Unauthorized();
                }
                await _userCollection.DeleteOneAsync(user => user.ID == id);
                return NoContent();
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
    }
}
