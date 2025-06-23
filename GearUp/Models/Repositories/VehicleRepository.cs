using GearUp.Models.Interfaces;
using GearUp.Models;                 // Vehicle model
using GearUp.Data;                  // ApplicationDbContext
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace GearUp.Models.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddVehicle(Vehicle vehicle)
        {
            vehicle.AvailabilityStatus = true;
            _context.Vehicles.Add(vehicle);
            return _context.SaveChanges() > 0;
        }
        public bool UpdateVehicle(Vehicle vehicle)
        {
            var existingVehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == vehicle.PlateNumber);
            if (existingVehicle != null)
            {
                existingVehicle.RentPerDay = vehicle.RentPerDay;
                // Optionally update other fields as needed
                _context.Entry(existingVehicle).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteVehicle(Vehicle vehicle)
        {
            var existingVehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == vehicle.PlateNumber);
            if (existingVehicle != null)
            {
                _context.Vehicles.Remove(existingVehicle);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public Vehicle GetVehicleById(int vehicleId)
        {
            var vehicle = _context.Vehicles.Find(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {vehicleId} was not found.");

            return vehicle;
        }

        public Vehicle GetVehicleByPlateNumber(string plateNumber)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == plateNumber);

            if (vehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {plateNumber} was not found.");

            return vehicle;
        }


        public IEnumerable<Vehicle> ReadAllVehicles()
        {
            return _context.Vehicles.ToList();  // ✅ Use 'Vehicles'
        }

        public IEnumerable<Vehicle> ReadLuxuryCars()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.Category, "Luxury"))
                .ToList();
        }

        public IEnumerable<Vehicle> ReadEconomyCars()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.Category, "Economy"))
                .ToList();
        }

        public IEnumerable<Vehicle> ReadSportsCars()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.Category, "Sports"))
                .ToList();
        }

        public IEnumerable<Vehicle> ReadCoasters()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.Category, "Coaster"))
                .ToList();
        }
        public bool BookVehicle(Vehicle vehicle)
        {
            var existingVehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == vehicle.PlateNumber);
            if (existingVehicle != null && existingVehicle.AvailabilityStatus == true)
            {
                existingVehicle.AvailabilityStatus = false; // Set to booked
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool ReturnedVehicle(Vehicle vehicle)
        {
            var existingVehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == vehicle.PlateNumber);
            if (existingVehicle != null && existingVehicle.AvailabilityStatus == false)
            {
                existingVehicle.AvailabilityStatus = true; // Set to available 
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public IEnumerable<Vehicle> LahoreCars()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.City, "Lahore"))
                .ToList();
        }
        public IEnumerable<Vehicle> IslamabadCars()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.City, "Islamabad"))
                .ToList();
        }
        public IEnumerable<Vehicle> KarachiCars()
        {
            return _context.Vehicles
                .Where(v => EF.Functions.Like(v.City, "Karachi"))
                .ToList();
        }

    }
}
