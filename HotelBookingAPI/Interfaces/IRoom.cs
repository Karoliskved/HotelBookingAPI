using hotelBooking.Models;
using HotelBookingAPI.Models;
using System.Collections;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoom
    {
        Task<List<Room>> GetAllRooms(int? limit);
        Task<List<Room>> GetRoomsByHotel(string? hotelID);
        Task<List<Room>> SortRooms(string[] atributes, string[] operators);
        Task<List<Room>> GetRoomsByMultiParam(string[] atributes, Object[] distances, string[] operators);
        Task<Room> GetRoomById(string? id);
        Task<string?> AddRoom(Room room);
        Task<Room?> UpdateRoomByID(string id, Room newRoom);
        Task<RoomPriceRange?> AddPriceInterval(Room room, RoomPriceRange roomPriceRange);
        Task<string?> DeleteRoomByID(string id);
        Task<BookedRoomInfo?> BookRoom(RoomBookingInfo bookingInfo, bool OnlyCalculatePrice);
        Task<List<BookedDateRange>?> ShowAvailableBookingDates(string id);
        Task<string?> CancelBooking(CancellationInfo cancellationInfo);
        Task<double?> CalculateAdditionalexpenses(string[] selectedExpenses, string id);
}
}
