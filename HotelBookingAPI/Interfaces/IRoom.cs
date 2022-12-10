using hotelBooking.Models;
using HotelBookingAPI.Models;
using System.Collections;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoom
    {
        Task<List<Room>> GetAllRooms();
        Task<List<Room>> GetRoomsByHotel(string? hotelID);
        Task<List<Room>> SortRooms(string[] atributes, string[] operators);
        Task<List<Room>> GetRoomsByMultiParam(string[] atributes, Object[] distances, string[] operators);

        Task<Room> GetRoomById(string? id);
        Task<string?> AddRoom(Room room);

        Task<Room?> UpdateRoomByID(string id, Room newRoom);

        Task<string?> AddPriceInterval(string id, RoomPriceRange roomPriceRange);
        Task<string?> DeleteRoomByID(string id);
        Task<BookedRoomInfo?> BookRoom(RoomBookingInfo bookingInfo);
        Task<List<BookedDateRange>?> ShowAvailableBookingDates(string id);
}
}
