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
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRooms();
            if (rooms is not null)
            {
                return Ok(rooms);
            }
            return NotFound("There are no rooms in the database.");

        }
        [HttpGet("/byHotel/{hotelID}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRoomsByHotel(string hotelID)
        {
            
            if (ObjectId.TryParse(hotelID, out _))
            {
                var rooms = await _roomService.GetRoomsByHotel(hotelID);
                if (rooms is not null)
                {
                    return Ok(rooms);
                }
                return NotFound($"Room with id: {hotelID} doesn't exist.");
            }
            return BadRequest($"Invalid id: {hotelID} provided.");
        }
        [HttpPost("multi/sort")]
        [AllowAnonymous]
        public async Task<IActionResult> SortHRooms([FromBody] FilterSort input)

        {
            string[]? atributes = input.Atributes;

            string[]? operators = input.Operators;
            var rooms = await _roomService.SortRooms(atributes, operators);
            if (rooms is not null)
            {
                return Ok(rooms);
            }
            return NotFound($"No rooms");
        }
        [HttpPost("multi/filter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomByMultiParam([FromBody] FilterInput input)
        {
            /*string[] atributes = { "geographicData.distToBeach", "geographicData.distToMountains" };
            double[] distances = { 26, 21 };*/
            /*
                     {
     "atributes": [
    "geographicData.distToBeach", "geographicData.distToCity"
  ],
  "distances": [
    26, 20
  ],
  "operators": [
    "Lt", "Gt"
  ]

     ***************
      "atributes": [
    "geographicData.distToBeach", "geographicData.distToBeach"
  ],
  "distances": [
    30, 10
  ],
  "operators": [
    "Lt", "Gt"
  ]
}*/
            string[]? atributes = input.Atributes;
            string[]? types = input.Types;
            Object[] ArrayOfObjects = new object[atributes.Length];
            var distances = input.Distances;
            for (int i = 0; i < atributes.Length; i++)
            {
                switch (types[i])
                {
                    case "double":
                        double inputVarInt = double.Parse(distances[i]);
                        ArrayOfObjects[i] = inputVarInt;
                        break;
                    case "string":
                        ArrayOfObjects[i] = distances[i];
                        break;
                    case "bool":
                        bool inputVarBool = bool.Parse(distances[i]);
                        ArrayOfObjects[i] = inputVarBool;
                        break;

                }
            }
            string[]? operators = input.Operators;
            var rooms = await _roomService.GetRoomsByMultiParam(atributes, ArrayOfObjects, operators);
            if (rooms is not null)
            {
                return Ok(rooms);
            }
            return NotFound($"No rooms");
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
        [HttpGet("/availableBookingDates/{id:length(24)}")]
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
        [HttpPost("/add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            await _roomService.AddRoom(room);
            return CreatedAtAction(nameof(AddRoom), new { id = room.ID }, room);
        }
        [HttpPost("/{id}/addPriceInterval")]
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
