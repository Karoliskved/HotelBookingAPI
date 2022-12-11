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
        public async Task<List<Room>> GetAllRooms(int? limit)
        {
            if (limit is null)
            {
                return await _roomCollection.Find(_ => true).ToListAsync();
            }
            return await _roomCollection.Find(_ => true).Limit(limit).ToListAsync();
        }
        public async Task<Room> GetRoomById(string? id)
        {
            return await _roomCollection.Find(room => room.RoomID == id).FirstOrDefaultAsync();
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
            await _roomCollection.ReplaceOneAsync(room => room.RoomID == id, newRoom);
            return newRoom;
        }

        public async Task<string?> DeleteRoomByID(string id)
        {
            await _roomCollection.DeleteOneAsync(room => room.RoomID == id);
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
            int days = bookingInfo.ToDate.Date.Subtract(bookingInfo.FromDate.Date).Days;
            if (days <= 0)
            {
                return null;
            }
            foreach (var bookedDate in room.BookedDates)
            {
                if (!(bookingInfo.FromDate.Date >= bookedDate.ToDate.Date || bookingInfo.ToDate.Date <= bookedDate.FromDate.Date))
                {
                    return null;
                }
            }
            decimal? price = 0;
            var tempDate = bookingInfo.FromDate.Date;
            foreach (var priceRange in room.PriceRanges)
            {
                if (tempDate >= priceRange.FromDate.Date && tempDate <= priceRange.ToDate.Date)
                {
                    int count = Math.Abs(tempDate.Subtract(priceRange.ToDate.Date).Days);
                    if (days == 0)
                    {
                        break;
                    }
                    if (count >= days)
                    {
                        price += days * priceRange.Price;
                        tempDate = tempDate.AddDays(days);
                        days -= days;
                        continue;
                    }
                    price += count * priceRange.Price;
                    days -= count;
                    tempDate = tempDate.AddDays(count);
                }
            }
            if (days != 0)
            {
                return null;
            }
            BookedRoomInfo bookedRoomInfo = new()
            {
                UserID = bookingInfo.UserID,
                RoomID = bookingInfo.RoomID,
                Price = price,
                FromDate = bookingInfo.FromDate,
                ToDate = bookingInfo.ToDate
            };
            FilterDefinition<Room> filter = Builders<Room>.Filter.Eq("RoomID", bookingInfo.RoomID);
            UpdateDefinition<Room> update = Builders<Room>.Update.AddToSet("bookedDates", bookedRoomInfo);
            await _roomCollection.UpdateOneAsync(filter, update);
            return bookedRoomInfo;
        }
        public async Task<string?> AddPriceInterval(string id, RoomPriceRange roomPriceRange)
        {
            FilterDefinition<Room> filter = Builders<Room>.Filter.Eq("RoomID", id);
            UpdateDefinition<Room> update = Builders<Room>.Update.AddToSet("priceRanges", roomPriceRange);
            await _roomCollection.UpdateOneAsync(filter, update);
            return null;
        }
        public async Task<List<BookedDateRange>?> ShowAvailableBookingDates(string id)
        {
            var room = await GetRoomById(id);
            DateTime now = DateTime.Now.Date;
            DateTime yearAfterNow = now.AddDays(365);
            List<BookedDateRange> availableBookingDates = new();
            if (room is not null)
            {
               room.BookedDates.Sort((x, y) => DateTime.Compare(x.FromDate, y.FromDate));
                DateTime intervalStart = now;
                DateTime intervalEnd = now;
            foreach (var bookedDate in room.BookedDates)
            {
                    if (intervalStart >= bookedDate.ToDate.Date || intervalEnd <= bookedDate.FromDate.Date)
                    {
                        intervalEnd = bookedDate.FromDate.Date;
                    }
                    else
                    {
                        intervalStart = bookedDate.ToDate.Date;
                        intervalEnd = bookedDate.ToDate.Date;
                        continue;
                    }
                    BookedDateRange availableBookingDate = new()
                    {
                        FromDate = intervalStart,
                        ToDate = intervalEnd
                    };
                    availableBookingDates.Add(availableBookingDate);
                    intervalStart = bookedDate.ToDate.Date;
                    intervalEnd = bookedDate.ToDate.Date;
            }
            if (room.BookedDates[room.BookedDates.Count-1].ToDate < yearAfterNow)
            {
                    BookedDateRange availableBookingDate = new()
                    {
                        FromDate = intervalStart,
                        ToDate = yearAfterNow
                    };
                    availableBookingDates.Add(availableBookingDate);
                }
            return availableBookingDates;
            }
            return null;

        }
        public async Task<string?> CancelBooking(CancellationInfo cancellationInfo)
        {
            var filter = Builders<Room>.Filter.Eq("RoomID", cancellationInfo.RoomID);
            var update = Builders<Room>.Update.PullFilter(x => x.BookedDates, y => y.UserID == cancellationInfo.UserID);
            await _roomCollection.UpdateOneAsync(filter, update);
            return null;
        }
    }
}
