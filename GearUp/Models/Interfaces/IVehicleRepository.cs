namespace GearUp.Models.Interfaces
{
    public interface IVehicleRepository
    {
        bool AddVehicle(Vehicle vehicle);
        bool UpdateVehicle(Vehicle vehicle);
        bool DeleteVehicle(Vehicle vehicle);
        Vehicle GetVehicleById(int vehicleId);
        Vehicle GetVehicleByPlateNumber(string platenumber);
        IEnumerable<Vehicle> ReadAllVehicles();
        IEnumerable<Vehicle> ReadLuxuryCars();
        IEnumerable<Vehicle> ReadEconomyCars();
        IEnumerable<Vehicle> ReadSportsCars();
        IEnumerable<Vehicle> ReadCoasters();
        bool BookVehicle(Vehicle vehicle);
        bool ReturnedVehicle(Vehicle vehicle);
        IEnumerable<Vehicle> LahoreCars();
        IEnumerable<Vehicle> IslamabadCars();
        IEnumerable<Vehicle> KarachiCars();

    }
}
