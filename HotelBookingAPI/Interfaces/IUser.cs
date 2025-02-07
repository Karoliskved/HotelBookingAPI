﻿using hotelBooking.Models;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IUser
    {
        Task<User?> Register(UserLogin user);
        Task<User?> RegisterAdmin(UserLogin user);
        Task<string?> Login(UserLogin user);
        Task<string?> UpdateUser(User user);
        Task<string?> DeleteUser(string userID);
        Task<User?> GetUserByID(string? ID);
        Task<List<User>> GetAllUsers();
        Task<string?> AddRoomToUser(BookedRoomInfo bookingInfo, bool OnlyCalculatePrice = false);
        Task<string?> CancelBooking(CancellationInfo cancellationInfo);
    }
}
