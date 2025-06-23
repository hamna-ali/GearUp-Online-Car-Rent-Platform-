namespace GearUp.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty; // Unique
        public string Description { get; set; } = string.Empty;
        public decimal RentPerDay { get; set; }
        public bool AvailabilityStatus { get; set; } = true;
        public string ImagePath { get; set; } = string.Empty;

        // Parameterless constructor required for model binding
        public Vehicle() { }

        // Optional full constructor
        public Vehicle(string brand, string model, string category, string city, string plateNumber, string description, decimal rentPerDay, string imagePath)
        {
            Brand = brand;
            Model = model;
            Category = category;
            PlateNumber = plateNumber;
            City = city;
            Description = description;
            RentPerDay = rentPerDay;
            ImagePath = imagePath;
        }
    }
}
