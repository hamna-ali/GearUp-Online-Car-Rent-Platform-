using GearUp.Models.Interfaces;
using GearUp.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GearUp.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public CartController(ICartRepository cartRepository, IVehicleRepository vehicleRepository)
        {
            _cartRepository = cartRepository;
            _vehicleRepository = vehicleRepository;
        }

        public IActionResult Index()
        {
            var cart = _cartRepository.GetCart(Request);
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int vehicleId)
        {
            _cartRepository.RemoveFromCart(Request, Response, vehicleId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int vehicleId, int noOfDays = 1, bool includeCarWash = false, bool includeCarDecor = false)
        {
            var vehicle = _vehicleRepository.GetVehicleById(vehicleId);
            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Vehicle not found.";
                return RedirectToAction("Index");
            }

            var (success, message) = await _cartRepository.AddToCartAsync(Request, Response, vehicle, noOfDays, includeCarWash, includeCarDecor);

            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("Details", "Vehicle", new { id = vehicleId });
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItem(int vehicleId, int noOfDays, bool includeCarWash, bool includeCarDecor)
        {
            var result = await _cartRepository.UpdateCartItemAsync(Request, Response, vehicleId, noOfDays, includeCarWash, includeCarDecor);

            if (result.success)
            {
                var updatedCart = _cartRepository.GetCart(Request);
                var item = updatedCart.CartItems.FirstOrDefault(i => i.Vehicle.VehicleID == vehicleId);

                if (item != null)
                {
                    return Json(new
                    {
                        success = true,
                        updatedTotal = updatedCart.TotalBill,
                        itemTotal = item.TotalPrice, 
                        //itemTotal = item.NoOfDays * item.Vehicle.RentPerDay,
                        vehicleId = item.Vehicle.VehicleID
                    });
                }

                return Json(new { success = false, message = "Updated item not found in cart." });
            }

            return Json(new { success = false, message = result.message });
        }

    }
}
