using Azure.Core;
using Azure;
using GearUp.Data;
using GearUp.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GearUp.Models.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private const string CartCookieKey = "CartData";

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Cart GetCart(HttpRequest request)
        {
            return CookieHelper.GetCookie<Cart>(request, CartCookieKey) ?? new Cart();
        }

        public void SaveCart(HttpResponse response, Cart cart)
        {
            CookieHelper.SetCookie(response, CartCookieKey, cart);
        }

        public void RemoveFromCart(HttpRequest request, HttpResponse response, int vehicleId)
        {
            var cart = GetCart(request);

            var itemToRemove = cart.CartItems.FirstOrDefault(i => i.Vehicle.VehicleID == vehicleId);
            if (itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);
                SaveCart(response, cart);
            }
        }
        public async Task<(bool success, string message)> AddToCartAsync(HttpRequest request, HttpResponse response, Vehicle vehicle, int noOfDays, bool includeCarWash, bool includeCarDecor)
        {
            try
            {
                var cart = GetCart(request);
                var existingItem = cart.CartItems.FirstOrDefault(i => i.Vehicle.VehicleID == vehicle.VehicleID);

                if (existingItem != null)
                {
                    existingItem.NoOfDays += noOfDays;
                    existingItem.IncludeCarWash |= includeCarWash;
                    existingItem.IncludeCarDecor |= includeCarDecor;
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        Vehicle = vehicle,
                        NoOfDays = noOfDays,
                        IncludeCarWash = includeCarWash,
                        IncludeCarDecor = includeCarDecor
                    });
                }

                SaveCart(response, cart);

                // Since no async work here, just return completed task:
                return await Task.FromResult((true, "Added to cart successfully."));
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add to cart: {ex.Message}");
            }
        }


        // Update existing cart item
        public async Task<(bool success, string message)> UpdateCartItemAsync(HttpRequest request, HttpResponse response, int vehicleId, int noOfDays, bool includeCarWash, bool includeCarDecor)
        {
            try
            {
                var cart = GetCart(request);
                var item = cart.CartItems.FirstOrDefault(i => i.Vehicle.VehicleID == vehicleId);

                if (item != null)
                {
                    item.NoOfDays = noOfDays;
                    item.IncludeCarWash = includeCarWash;
                    item.IncludeCarDecor = includeCarDecor;
                    SaveCart(response, cart);
                    return await Task.FromResult((true, "Cart updated successfully."));
                }
                return (false, "Item not found in cart.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update cart: {ex.Message}");
            }
        }
        public async Task ClearCartAsync(HttpResponse response)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1),
                IsEssential = true
            };

            response.Cookies.Delete("CartData");
            await Task.CompletedTask;
        }


    }
}
