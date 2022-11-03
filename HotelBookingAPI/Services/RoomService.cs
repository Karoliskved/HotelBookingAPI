using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
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
        public async Task<Room> GetRoomById(string? id)
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

        public async Task<BookedRoomInfo?> BookRoom(RoomBookingInfo bookingInfo)
        {
            if (bookingInfo.FromDate.Date < DateTime.Today.Date)
            {
                return null;
            }
            var room = await GetRoomById(bookingInfo.RoomID);
            if (room is null)
            {
                return null;
            }
            int days = bookingInfo.ToDate.Date.Subtract(bookingInfo.FromDate).Days;
            foreach (var bookedDate in room.BookedDates)
            {
                if (bookingInfo.FromDate.Date >= bookedDate.FromDate.Date && bookingInfo.FromDate.Date <= bookedDate.ToDate.Date)
                {
                    return null;
                }
                if (bookingInfo.ToDate.Date >= bookedDate.FromDate.Date && bookingInfo.ToDate.Date <= bookedDate.ToDate.Date)
                {
                    return null;
                }
            }
            decimal price = 0;
            foreach (var priceRange in room.PriceRanges)
            {
                if (bookingInfo.FromDate.Date >= priceRange.FromDate.Date && bookingInfo.FromDate.Date <= priceRange.ToDate.Date)
                {
                    int count = 0;
                    var startDate = priceRange.FromDate.Date;
                    while (startDate != priceRange.ToDate.Date && days != 0)
                    {
                        count++;
                        startDate.AddDays(1);
                        days--;
                    }
                    price += count * priceRange.Price;
                    if (days == 0)
                    {
                        break;
                    }
                }
            }
            if (days != 0)
            {
                return null;
            }
            BookedRoomInfo bookedRoomInfo = new()
            {
                Price = price,
                FromDate = bookingInfo.FromDate,
                ToDate = bookingInfo.ToDate
            };
            FilterDefinition<Room> filter = Builders<Room>.Filter.Eq("ID", bookingInfo.RoomID);
            UpdateDefinition<Room> update = Builders<Room>.Update.AddToSet("BookedDates", bookedRoomInfo);
            await _roomCollection.UpdateOneAsync(filter, update);
            return bookedRoomInfo;
        }
    public async Task<string?> AddPriceInterval(string id, RoomPriceRange roomPriceRange)
    {
            FilterDefinition<Room> filter = Builders<Room>.Filter.Eq("ID", id);
            UpdateDefinition<Room> update = Builders<Room>.Update.AddToSet("priceRanges", roomPriceRange);
            await _roomCollection.UpdateOneAsync(filter, update);
            return null;
        }
    }
}
