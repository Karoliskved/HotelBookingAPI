namespace HotelBookingAPI.Models
{
    public class FilterInput
    {
        public string[]? Atributes { get; set; }
        public double[]? Distances { get; set; }
        public string[]? Operators { get; set; }
    }
}
