using hotelBooking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AddRoomController : ControllerBase
    {
        private IMongoCollection<Room> _addRoomCollection;

        public AddRoomController(IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _addRoomCollection = database.GetCollection<Room>("Rooms");
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Room>> Get(string id)
        {
            var room = await _addRoomCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (room is null)
            {
                return NotFound();
            }

            return room;
        }
        [HttpPost]
        public async Task<IActionResult> Post (Room newRoom)
        {
            newRoom.RoomNumber = 1;
            _addRoomCollection.InsertOne(newRoom);
            return CreatedAtAction(nameof(Get), new { id = newRoom.Id }, newRoom);
        }
        
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Room updatedRoom)
        {

            var room=_addRoomCollection.Find(x => x.Id == id).FirstOrDefault();
            if (room is null)
            {
                return NotFound();
            }

            updatedRoom.Id = room.Id;
           await _addRoomCollection.ReplaceOneAsync(x => x.Id == id, updatedRoom);

            return NoContent();
        }
        
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {



            await _addRoomCollection.DeleteOneAsync(x => x.Id == id);

            return NoContent();
        }
    }
}
