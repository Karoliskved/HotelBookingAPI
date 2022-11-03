using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelBookingAPI.Services
{
    public class UserService : IUser
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _userCollection;

        public UserService(IConfiguration configuration, IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _userCollection = database.GetCollection<User>("Users");
            _configuration = configuration;
        }

        public async Task<User?> Register(UserLogin user)
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
                return newUser;
            }
            return null;
        }
        public async Task<string?> Login(UserLogin user)
        {
            var result = await GetUserByUsername(user);
            if (result is null)
            {
                return null;
            }
            if (result.Password == user.Password)
            {
                User newUser = new()
                {
                    Username = user.Username,
                    Password = user.Password
                };
                string token = CreateToken(newUser);
                return token;
            }
            return null;
        }
        public async Task<User?> GetUserByUsername(UserLogin user)
        {
            var result = await _userCollection.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
            if (result is null)
            {
                return null;
            }
            return result;
        }
        public async Task<User?> GetUserByID(string? ID)
        {
            var result = await _userCollection.Find(u => u.ID == ID).FirstOrDefaultAsync();
            if (result is null)
            {
                return null;
            }
            return result;
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _userCollection.Find(_ => true).ToListAsync();
        }
        public async Task<string?> DeleteUser(string userID)
        {
            await _userCollection.DeleteOneAsync(user => user.ID == userID);
            return null;
        }
        public async Task<string?> UpdateUser(User user)
        {
            await _userCollection.ReplaceOneAsync(u => u.ID == user.ID, user);
            return null;
        }
        public async Task<string?> AddRoomToUser(BookedRoomInfo bookedRoomInfo)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("ID", bookedRoomInfo.UserID);
            UpdateDefinition<User> update = Builders<User>.Update.AddToSet("bookedRooms", bookedRoomInfo);
            await _userCollection.UpdateOneAsync(filter, update);
            return null;
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
