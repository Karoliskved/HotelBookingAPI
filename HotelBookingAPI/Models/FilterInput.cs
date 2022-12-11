using System.Collections;

namespace HotelBookingAPI.Models
{
    public class FilterInput
    {
        public string[]? Atributes { get; set; }
        public string[]? Distances { get; set; }
        public string[]? Types { get; set; }
        public string[]? Operators { get; set; }
    }
}
