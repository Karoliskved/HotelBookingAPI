using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using MongoDB.Driver;

namespace HotelBookingAPI.Services
{
    public class RoomService : IRoom
    {
        private readonly IMongoCollection<Room> _roomCollection;

        public RoomService(IMongoClient client)
        {
            var database = client.GetDatabase("HotelRoomsDB");
            _roomCollection = database.GetCollection<Room>("Rooms");
        }
        public async Task<List<Room>> GetAllRooms()
        {
            return await _roomCollection.Find(_ => true).ToListAsync();
        }
        public async Task<Room> GetRoomById(string id)
        {
            return await _roomCollection.Find(room => room.ID == id).FirstOrDefaultAsync();
        }

        public async Task<string?> AddRoom(Room room)
        {
            await _roomCollection.InsertOneAsync(room);
            return null;
        }

        public async Task<Room?> UpdateRoomByID(string id, Room newRoom)
        {
            var room = await GetRoomById(id);
            if (room is null)
            {
                return null;
            }
            await _roomCollection.ReplaceOneAsync(room => room.ID == id, newRoom);
            return newRoom;
        }

        public async Task<string?> DeleteRoomByID(string id)
        {
            await _roomCollection.DeleteOneAsync(room => room.ID == id);
            return null;
        }

        public async Task<string?> BookRoom(string id)
        {
            FilterDefinition<Room> filter2 = Builders<Room>.Filter.Eq("ID", id);
            UpdateDefinition<Room> update2 = Builders<Room>.Update.Set("Booked", true);
            await _roomCollection.UpdateOneAsync(filter2, update2);
            return null;
        }
    }
}
