namespace GearUp.Models.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCart(HttpRequest request);
        void SaveCart(HttpResponse response, Cart cart);
        void RemoveFromCart(HttpRequest request, HttpResponse response, int vehicleId);
        Task<(bool success, string message)> UpdateCartItemAsync(HttpRequest request, HttpResponse response, int vehicleId, int noOfDays, bool includeCarWash, bool includeCarDecor);
        Task<(bool success, string message)> AddToCartAsync(HttpRequest request, HttpResponse response, Vehicle vehicle, int noOfDays, bool includeCarWash, bool includeCarDecor);
        Task ClearCartAsync(HttpResponse response);

    }
}
