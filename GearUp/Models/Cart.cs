namespace GearUp.Models
{
    public class Cart
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public int TotalItems => CartItems.Count;
        public decimal TotalBill
        {
            get
            {
                return CartItems?.Sum(item => item.TotalPrice) ?? 0;
            }
        }
    }
}
