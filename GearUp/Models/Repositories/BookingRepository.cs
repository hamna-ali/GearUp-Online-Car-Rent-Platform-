using GearUp.Data;
using GearUp.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GearUp.Models.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartRepository _cartRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public BookingRepository(ApplicationDbContext context, ICartRepository cartRepository, IVehicleRepository vehicleRepository)
        {
            _context = context;
            _cartRepository = cartRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task CheckoutAsync(HttpRequest request, HttpResponse response, string username)
        {
            var cart = _cartRepository.GetCart(request);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return;

            foreach (var item in cart.CartItems)
            {
                // Mark vehicle as booked in Vehicle table
                bool booked = _vehicleRepository.BookVehicle(item.Vehicle); // synchronous version you have

                if (!booked)
                {
                    // Optionally handle failure to book vehicle (skip or throw)
                    continue;
                }

                var booking = new Booking
                {
                    Username = username,
                    VehiclePlateNumber = item.Vehicle.PlateNumber,
                    BookingDateTime = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(item.NoOfDays)
                };

                await _context.Bookings.AddAsync(booking);
            }

            await _context.SaveChangesAsync();

            // Clear the cart cookie after successful booking
            await _cartRepository.ClearCartAsync(response);

        }


        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task<List<Booking>> ShowBookingsAsync(string username)
        //{
        //    return await Task.FromResult(_context.Bookings
        //        .Where(b => b.Username == username)
        //        .OrderByDescending(b => b.BookingDateTime)
        //        .ToList());
        //}
        public async Task<List<Booking>> ShowBookingsAsync()
        {
            return await _context.Bookings
                .OrderByDescending(b => b.BookingDateTime)
                .ToListAsync();
        }

    }
}
