using hotelBooking.Models;
using HotelBookingAPI.Models;
namespace HotelBookingAPI.Interfaces
{
    public interface IHotel
    {
        Task<List<Hotel>> GetAllHotels();
        Task<Hotel> GetHotelById(string? id);
        Task<string?> AddHotel(Hotel hotel);
        Task<Hotel> GetHotelByDistToBeach(double? id);
        Task<List<Hotel>> SortHotels(string[] atributes, string[] operators);
        Task<List<Hotel>> GetHotelByMultiParam(string[] atributes, double[] distances, string[] operators);
    }
}
