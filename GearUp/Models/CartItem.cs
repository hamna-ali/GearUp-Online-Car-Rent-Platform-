namespace GearUp.Models
{
    public class CartItem
    {

        public Vehicle? Vehicle { get; set; } // Vehicle object
        public int NoOfDays { get; set; }
        public bool IncludeCarWash { get; set; }
        public decimal CarWashPrice { get; set; }

        public bool IncludeCarDecor { get; set; }
        public decimal CarDecorPrice { get; set; }

        // Computed Total
        public decimal TotalPrice
        {
            get
            {
                decimal basePrice = Vehicle?.RentPerDay * NoOfDays ?? 0;
                decimal addonPrice = 0;

                if (IncludeCarWash) addonPrice += 500;
                if (IncludeCarDecor) addonPrice += 2000;

                return basePrice + addonPrice;
            }
        }
    }

}
