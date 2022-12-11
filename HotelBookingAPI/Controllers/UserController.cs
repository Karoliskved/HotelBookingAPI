using hotelBooking.Models;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;


namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserService _userService;
        private readonly RoomService _roomService;
        public UserController(UserService userService, RoomService roomService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roomService = roomService;
            _context = httpContextAccessor;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<List<User>> GetUsers() => await _userService.GetAllUsers();

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
                var user = await _userService.GetUserByID(id);
                if (user is null)
                {
                    return NotFound($"User with id: {id} doesn't exist.");
                }
                if (user.Username != username)
                {
                    return Unauthorized();
                }
                return Ok(user);
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [HttpPost("bookHotelRoom")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> BookHotelRoomByID([FromBody] RoomBookingInfo bookingInfo)
        {
            if (ObjectId.TryParse(bookingInfo.RoomID, out _) || ObjectId.TryParse(bookingInfo.UserID, out _))
            {
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var result = await _userService.GetUserByID(bookingInfo.UserID);
                if (result is null)
                {
                    return NotFound("User doesn't exist.");
                }
                if (result.Username != username)
                {
                    return Unauthorized();
                }
                var room = await _roomService.GetRoomById(bookingInfo.RoomID);
                if (room is null)
                {
                    return NotFound($"Room with id: {bookingInfo.RoomID} doesn't exist.");
                }
                var bookedRoomInfo = await _roomService.BookRoom(bookingInfo);
                if (bookedRoomInfo is null)
                {
                    return BadRequest("Can't book the room with provided dates.");
                }
                await _userService.AddRoomToUser(bookedRoomInfo);
                return Ok(bookedRoomInfo);
            }
            return BadRequest("Invalid id provided.");
        }
        [HttpGet("{id}/getBookedRooms")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ShowBookedRooms(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {

                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var user = await _userService.GetUserByID(id);
                if (user is null)
                {
                    return NotFound($"User with id: {id} doesn't exist.");
                }
                if (user.Username != username)
                {
                    return Unauthorized();
                }
                var result = _userService.GetBookedRooms(id);
                if (result is null)
                {
                    return StatusCode(500);
                }
                return Ok(result.Result.ToArray());
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (ObjectId.TryParse(id, out _))
            {
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var result = await _userService.GetUserByID(id);
                if (result is null)
                {
                    return NotFound("User doesn't exist.");
                }
                if (result.Username != username)
                {
                    return Unauthorized();
                }
                await _userService.UpdateUser(user);
                return NoContent();
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
                string? username = _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Unauthorized();
                }
                var user = await _userService.GetUserByID(id);
                if (user is null)
                {
                    return NotFound("User doesn't exist.");
                }
                if (user.Username != username)
                {
                    return Unauthorized();
                }
                await _userService.DeleteUser(id);
                return NoContent();
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [HttpPost("/{id}/CalculateExtras")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculateExtraCosts(string id, [FromBody] string[] additionalExpenses)
        {
            var room = await _roomService.GetRoomById(id);
            if (room is null)
            {
                return NotFound($"Room with id: {id} doesn't exist.");
            }
            var extracost=await  _roomService.CalculateAdditionalexpenses(additionalExpenses, id);
            return Ok(extracost);
        }
    }
}
