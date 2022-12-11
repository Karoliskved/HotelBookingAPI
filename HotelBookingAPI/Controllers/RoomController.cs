using hotelBooking.Models;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        
        public readonly RoomService _roomService;
        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRooms([FromBody] int? limit)
        {
            var rooms = await _roomService.GetAllRooms(limit);
            if (rooms is not null)
            {
                return Ok(rooms);
            }
            return NotFound("There are no rooms in the database.");
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomById(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
            var room = await _roomService.GetRoomById(id);
            if(room is not null)
            {
                return Ok(room);
            }
            return NotFound($"Room with id: {id} doesn't exist.");
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [HttpGet("{id:length(24)}/availableBookingDates")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableBookingDates(string id)
        {
            var result = await _roomService.ShowAvailableBookingDates(id);
            if(result is  null)
            {
                return NotFound("Room with id: {id} doesn't exist.");
            }
            return Ok(result);
        }
        [HttpPost("add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            await _roomService.AddRoom(room);
            return CreatedAtAction(nameof(AddRoom), new { id = room.RoomID }, room);
        }
        [HttpPost("{id}/addPriceInterval")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> AddPriceInterval(string id, [FromBody] RoomPriceRange roomPriceRange)
        {
            var room = await _roomService.GetRoomById(id);
            if (room is null)
            {
                return NotFound($"Room with id: {id} doesn't exist.");
            }
            await _roomService.AddPriceInterval(id, roomPriceRange);
            return NoContent();
        }
        [HttpPut("{id:length(24)}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> UpdateRoomByID(string id, [FromBody] Room newRoom)
        {
            var room = await _roomService.UpdateRoomByID(id, newRoom);
            if (room is null)
            {
                return NotFound($"Room with id: {id} doesn't exist.");
            }
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            await _roomService.DeleteRoomByID(id);
            return NoContent();
        }
    }
}
