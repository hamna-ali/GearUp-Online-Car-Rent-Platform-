namespace GearUp.Models.Interfaces
{
    public interface IBookingRepository
    {
        Task CheckoutAsync(HttpRequest request, HttpResponse response, string username);
        Task DeleteBookingAsync(int bookingId);
        Task<List<Booking>> ShowBookingsAsync();
    }
}
