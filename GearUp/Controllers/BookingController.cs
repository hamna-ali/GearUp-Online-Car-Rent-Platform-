using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using GearUp.Models; 
using GearUp.Models.Repositories;
using GearUp.Models.Interfaces; 

namespace GearUp.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(IBookingRepository bookingRepository, UserManager<ApplicationUser> userManager, ICartRepository cartRepository)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            // Optional: fetch FullName if it exists, or fallback
            var fullName = User.FindFirst("FullName")?.Value ?? User.Identity.Name;

            await _bookingRepository.CheckoutAsync(Request, Response, fullName);

            TempData["BookingSuccess"] = "Your booking was successful! 🎉";

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookingId)
        {
            await _bookingRepository.DeleteBookingAsync(bookingId);
            return RedirectToAction("ShowBookings");
        }

        [HttpGet]
        [Authorize(Policy = "IsAdmin")]// Ensure only admins can access this action
        public async Task<IActionResult> ShowBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(); // Unauthorized if user is null (though should be handled by Authorize attribute)

            // Assuming FullName or Username is used for admin identification
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
                return Unauthorized(); // Double-check admin role (optional, if using Authorize(Roles = "Admin"))

            var bookings = await _bookingRepository.ShowBookingsAsync();
            return View(bookings);
        }

    }
}

