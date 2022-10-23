using hotelBooking.Models;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<User> _userCollection;

        public AuthController(IConfiguration configuration, IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _userCollection = database.GetCollection<User>("Users");
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<ActionResult<User>> Register([FromBody] UserLogin user)
        {
            var result = await _userCollection.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
            if (result is null)
            {
                User newUser = new()
                {
                    Username = user.Username,
                    Password = user.Password
                };
                await _userCollection.InsertOneAsync(newUser);
                return CreatedAtAction(nameof(Register), new { id = newUser.ID }, newUser);
            }
            return BadRequest("User already exists.");
        }
        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            var result = await _userCollection.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
            if (result is null)
            {
                return BadRequest("User doesn't exist.");
            }
            if (result.Password == user.Password)
            {
                User newUser = new()
                {
                    Username = user.Username,
                    Password = user.Password
                };
                string token = CreateToken(newUser);
                return Ok(token);
            }
            return BadRequest("Wrong password.");

        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtTokenInfo:SecretKey").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtTokenInfo:Issuer"],
                audience: _configuration["JwtTokenInfo:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
