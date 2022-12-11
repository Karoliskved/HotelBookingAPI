using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

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
                    Password = HashPassword(user.Password),
                    Role = Role.User
                };
                await _userCollection.InsertOneAsync(newUser);
                return newUser;
            }
            return null;
        }
        public async Task<User?> RegisterAdmin(UserLogin user)
        {
            var result = await _userCollection.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
            if (result is null)
            {
                User newUser = new()
                {
                    Username = user.Username,
                    Password = HashPassword(user.Password),
                    Role = Role.Administrator
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
            if (VerifyPassword(user.Password,result.Password))
            {
                User newUser = new()
                {
                    Username = user.Username,
                    Password = HashPassword(user.Password),
                    Role = result.Role
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
            var result = await _userCollection.Find(u => u.UserID == ID).FirstOrDefaultAsync();
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
            await _userCollection.DeleteOneAsync(user => user.UserID == userID);
            return null;
        }
        public async Task<string?> UpdateUser(User user)
        {
            await _userCollection.ReplaceOneAsync(u => u.UserID == user.UserID, user);
            return null;
        }
        public async Task<string?> AddRoomToUser(BookedRoomInfo bookedRoomInfo, bool OnlyCalculatePrice)
        {
            if (!OnlyCalculatePrice)
            {
                FilterDefinition<User> filter = Builders<User>.Filter.Eq("UserID", bookedRoomInfo.UserID);
                UpdateDefinition<User> update = Builders<User>.Update.AddToSet("bookedRooms", bookedRoomInfo);
                await _userCollection.UpdateOneAsync(filter, update);
            }
            return null;
        }
        public async Task<string?> CancelBooking(CancellationInfo cancellationInfo)
        {
            var filter = Builders<User>.Filter.Eq("UserID",cancellationInfo.UserID);
            var update = Builders<User>.Update.PullFilter(x => x.BookedRooms, y => y.RoomID == cancellationInfo.RoomID);
            await _userCollection.UpdateOneAsync(filter, update);
            return null;
        }
        public async Task<List<BookedRoomInfo>> GetBookedRooms(string id)
        {
            var filter = Builders<User>.Filter.Eq("UserID", id);
            var projection = Builders<User>.Projection.Include("bookedRooms");
            var options = new FindOptions<User> { Projection = projection };
            var result = await _userCollection.FindAsync(filter, options);
            var array = result.ToList();
            return array[0].BookedRooms;
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role,user.Role.ToString())
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
        private static string HashPassword(string password, byte[]? salt = null, bool needsOnlyHash = false)
        {
            if (salt == null || salt.Length != 16)
            {
                salt = new byte[128 / 8];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            if (needsOnlyHash) return hashed;
            return $"{hashed}:{Convert.ToBase64String(salt)}";
        }

        private static bool VerifyPassword(string password, string passwordInDB)
        {
            var passwordAndSalt = passwordInDB.Split(':');
            if (passwordAndSalt == null || passwordAndSalt.Length != 2)
                return false;
            var salt = Convert.FromBase64String(passwordAndSalt[1]);
            if (salt == null)
                return false;
            var hashOfpasswordToCheck = HashPassword(password, salt, true);
            if (String.Compare(passwordAndSalt[0], hashOfpasswordToCheck) == 0)
            {
                return true;
            }
            return false;
        }
    }
}
