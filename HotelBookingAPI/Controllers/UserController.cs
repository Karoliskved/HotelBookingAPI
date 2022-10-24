using hotelBooking.Models;
using HotelBookingAPI.Services;
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
        private readonly IHttpContextAccessor _context;
        private readonly UserService _userService;
        private readonly RoomService _roomService;
        public UserController(UserService userService, RoomService roomService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roomService = roomService;
            _context = httpContextAccessor;
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ("Administrator")]
        [HttpGet]
        public async Task<List<User>> GetUsers() =>  await _userService.GetAllUsers();
       
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
                if(user is null)
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
                var result = await _userService.GetUserByID(user.ID);
                if (result is null)
                {
                    return NotFound("User doesn't exist.");
                }
                if (result.Username != username)
                {
                    return Unauthorized();
                }
                var room = await _roomService.GetRoomById(user.ID);
                if(room is null)
                {
                    return NotFound($"Room with id: {id} doesn't exist.");
                }
                    await _roomService.BookRoom(id);
                    room = await _roomService.GetRoomById(id);
                    await _userService.AddRoomToUser(user, room);
                    return NoContent();
               
                }
                return BadRequest($"Invalid id provided.");
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
                var user = await _userService.GetUserByID(id);
                if(user is null)
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
    }
}
