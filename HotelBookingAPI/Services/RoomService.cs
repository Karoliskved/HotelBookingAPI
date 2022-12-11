using hotelBooking.Models;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using MongoDB.Driver;
using System.Collections;

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
        public async Task<List<Room>> GetRoomsByHotel(string? hotelID)
        {
            return await _roomCollection.Find(room => room.HotelID == hotelID).ToListAsync();
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
        public async Task<List<Room>> SortRooms(string[] atributes, string[] operators)
        {
            var bldr = Builders<Room>.Sort;
            int i = 0;
            var sortDefinitions = atributes.Select(x =>
            {

                SortDefinition<Room> sortDef;
                if (operators[i] == "1")
                {
                    sortDef = bldr.Descending(x);
                }
                else if (operators[i] == "0")
                {
                    sortDef = bldr.Ascending(x);
                }
                else
                {
                    sortDef = null;
                }
                i++;
                return sortDef;
            });
            /* for (int i = 0; i < atributes.Length; i++)
             {
                 sortString += atributes[i] + ": " + operators[i];
                 if (i != atributes.Length - 1)
                 {
                     sortString += ", ";
                 }
             }
             sortString += "}";*/
            var sortDef = bldr.Combine(sortDefinitions);

            return await _roomCollection.Find(_ => true).Limit(100).Sort(sortDef).ToListAsync();
        }
        public async Task<List<Room>> GetRoomsByMultiParam(string[] atributes, Object[] distances, string[] operators)
        {
            var builder = Builders<Room>.Filter;
            var filter = builder.Empty;
            for (int i = 0; i < atributes.Length; i++)
            {
                switch (operators[i])
                {
                    case "Lt":
                        filter = filter & builder.Lt(atributes[i], distances[i]);
                        break;
                    case "Gt":
                        filter = filter & builder.Gt(atributes[i], distances[i]);
                        break;
                    case "Eq":
                        filter = filter & builder.Eq(atributes[i], distances[i]);
                        break;
                }



                // _tempHotelcollection= _tempHotelcollection.Find(hotel =>  hotel.GeographicData.GetType().GetProperty("DistToShop").GetValue(hotel.GeographicData, null) == distances[i]).FirstOrDefaultAsync();
            }
            return await _roomCollection.Find(filter).ToListAsync();
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public async Task<double?> CalculateAdditionalexpenses(string[] selectedExpenses, string id) {
            var room = await GetRoomById(id);
            double finalExpense = 0;
            foreach (var expense in selectedExpenses)
            {

                finalExpense+=(Double)GetPropValue(room.AditionalPurchases, expense);
            }  
            return finalExpense;
        }
    }
}

