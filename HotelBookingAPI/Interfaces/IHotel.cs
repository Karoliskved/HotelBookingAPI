using hotelBooking.Models;
using HotelBookingAPI.Models;
using System.Collections;

namespace HotelBookingAPI.Interfaces
{
    public interface IHotel
    {
        Task<List<Hotel>> GetAllHotels();
        Task<Hotel> GetHotelById(string? id);
        Task<string?> AddHotel(Hotel hotel);
        Task<Hotel?> UpdateHotelByID(string id, Hotel newHotel);
        Task<string?> DeleteHotelByID(string id);
        Task<Hotel> GetHotelByDistToBeach(double? id);
        Task<List<Hotel>> SortHotels(string[] atributes, string[] operators);
        Task<List<Hotel>> GetHotelByMultiParam(string[] atributes, Object[] distances, string[] operators);
    }
}
