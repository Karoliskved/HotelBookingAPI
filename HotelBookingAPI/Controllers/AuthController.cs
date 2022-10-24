using hotelBooking.Models;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<ActionResult<User>> Register([FromBody] UserLogin user)
        {
            var newUser = await _userService.Register(user);
            if (newUser is null)
            {
                return BadRequest("User already exists.");
            }
            return CreatedAtAction(nameof(Register), new { id = newUser.ID }, newUser);
        }
        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLogin user)
        {
            var result = await _userService.Login(user);
            if (result is null)
            {
                return BadRequest("Wrong username or password.");
            }
            return result;

        }
    }
}
