using hotelBooking.Models;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IUser
    {
        Task<User?> Register(UserLogin user);
        Task<string?> Login(UserLogin user);
        Task<string?> UpdateUser(User user);
        Task<string?> DeleteUser(string userID);
        Task<List<User>> GetAllUsers();
        Task<string?> AddRoomToUser(User user, Room room);
    }
}
