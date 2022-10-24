using hotelBooking.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoom
    {
        Task<List<Room>> GetAllRooms();
        Task<Room> GetRoomById(string id);
        Task<string?> AddRoom(Room room);

        Task<Room?> UpdateRoomByID(string id, Room newRoom);

        Task<string?> DeleteRoomByID(string id);
        Task<string?> BookRoom(string id);


}
}
