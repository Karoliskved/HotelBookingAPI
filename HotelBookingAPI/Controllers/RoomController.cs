using hotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private IMongoCollection<Room> _roomCollection;

        public RoomController(IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _roomCollection = database.GetCollection<Room>("Rooms");
        }
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            var rooms = _roomCollection.Find(_ => true).ToList();
            if (rooms != null)
            {
                return Ok(rooms);
            }
            return NotFound("There are no rooms in the database.");
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomById(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
            var room = await _roomCollection.Find(room => room.ID == id).FirstOrDefaultAsync();
            if(room != null)
            {
                return Ok(room);
            }
            return NotFound($"Room with id: {id} doesn't exist.");
            }
            return BadRequest($"Invalid id: {id} provided.");
        }

        [HttpPost("/add")]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            await _roomCollection.InsertOneAsync(room);
            return CreatedAtAction(nameof(AddRoom), new { id = room.ID }, room);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateRoom(string id, [FromBody] Room newRoom)
        {
            var room = await _roomCollection.Find(room => room.ID == id).FirstOrDefaultAsync();
            if (room is null)
            {
                return NotFound();
            }
            await _roomCollection.ReplaceOneAsync(room => room.ID == id, newRoom);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            await _roomCollection.DeleteOneAsync(room => room.ID == id);
            return NoContent();
        }
    }
}
