using hotelBooking.Models;
using HotelBookingAPI.Models;
namespace HotelBookingAPI.Interfaces
{
    public interface IHotel
    {
        Task<List<Hotel>> GetAllHotels();
        Task<Hotel> GetHotelById(string? id);
        Task<Hotel> GetHotelByDistToBeach(double? id);
        Task<List<Hotel>> GetHotelByClosestTotheBeach();
        Task<List<Hotel>> GetHotelByMultiParam(string[] atributes, double[] distances);
    }
}
