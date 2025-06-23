using GearUp.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GearUp.Controllers
{
    public class CityController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;

        public CityController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }
        public IActionResult Lahore()
        {
            var lahoreCars = _vehicleRepository.LahoreCars();
            return View(lahoreCars);
        }
        public IActionResult Karachi()
        {
            var karachiCars = _vehicleRepository.KarachiCars();
            return View(karachiCars);
        }
        public IActionResult Islamabad()
        {
            var islamabadCars = _vehicleRepository.IslamabadCars();
            return View(islamabadCars);
        }
    }
}
