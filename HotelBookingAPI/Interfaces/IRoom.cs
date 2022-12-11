using hotelBooking.Models;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoom
    {
        Task<List<Room>> GetAllRooms(int? limit);
        Task<Room> GetRoomById(string? id);
        Task<string?> AddRoom(Room room);

        Task<Room?> UpdateRoomByID(string id, Room newRoom);

        Task<string?> AddPriceInterval(string id, RoomPriceRange roomPriceRange);
        Task<string?> DeleteRoomByID(string id);
        Task<BookedRoomInfo?> BookRoom(RoomBookingInfo bookingInfo);
        Task<List<BookedDateRange>?> ShowAvailableBookingDates(string id);
        Task<string?> CancelBooking(CancellationInfo cancellationInfo);
}
}
