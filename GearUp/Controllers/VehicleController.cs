using GearUp.Models.Interfaces;
using GearUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace GearUp.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;


        //public VehicleController(IVehicleRepository vehicleRepository, IHubContext<NotificationHub> hubContext)
        //{
        //    _vehicleRepository = vehicleRepository;
        //    _hubContext = hubContext;
        //}
        // GET: Vehicle/AllVehicles
        public VehicleController(IVehicleRepository vehicleRepository,
                         IHubContext<NotificationHub> hubContext,
                         IRazorViewEngine viewEngine,
                         ITempDataProvider tempDataProvider,
                         IServiceProvider serviceProvider)
        {
            _vehicleRepository = vehicleRepository;
            _hubContext = hubContext;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public IActionResult AllVehicles()
        {
            var allVehicles = _vehicleRepository.ReadAllVehicles();
            return View(allVehicles); 
        }
       
        public IActionResult Details(int id)
        {
            var vehicle = _vehicleRepository.GetVehicleById(id);
            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Vehicle not found!";
                return RedirectToAction("AllVehicles");
            }
            return View(vehicle);
        }

       
        public IActionResult Luxury()
        {
            var luxuryCars = _vehicleRepository.ReadLuxuryCars();
            return View(luxuryCars); 
        }

  
        public IActionResult Economy()
        {
            var economyCars = _vehicleRepository.ReadEconomyCars();
            return View(economyCars); 
        }

       
        public IActionResult Sports()
        {
            var sportsCars = _vehicleRepository.ReadSportsCars();
            return View(sportsCars); 
        }

       
        public IActionResult Coaster()
        {
            var coasters = _vehicleRepository.ReadCoasters();
            return View(coasters); 
        }

       
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View(new Vehicle()); 
        }
     
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Add(Vehicle vehicle, IFormFile image)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (image != null && image.Length > 0)
        //        {
        //            // Save the image to a specific folder in the project
        //            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pics");
        //            if (!Directory.Exists(uploadPath))
        //            {
        //                Directory.CreateDirectory(uploadPath);
        //            }

        //            // Save the file with its original name
        //            var filePath = Path.Combine(uploadPath, image.FileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                image.CopyTo(stream);
        //            }

        //            // Assign the saved image file name to the vehicle's ImagePath property
        //            vehicle.ImagePath = image.FileName;
        //        }

        //        bool isAdded = _vehicleRepository.AddVehicle(vehicle);
        //        if (isAdded)
        //        {
        //            TempData["SuccessMessage"] = "Vehicle added successfully!";
        //            return RedirectToAction("Add"); // Redirect to the Luxury view after adding
        //        }
        //        TempData["ErrorMessage"] = "Failed to add vehicle!";
        //        return View(vehicle); // Show form again if not valid
        //    }

        //    TempData["ErrorMessage"] = "Failed to add vehicle! Invalid data.";
        //    return View(vehicle); // Show form again if not valid
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Vehicle vehicle, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pics");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, image.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    vehicle.ImagePath = image.FileName;
                }

                bool isAdded = _vehicleRepository.AddVehicle(vehicle);
                if (isAdded)
                {
                  
                    var html = await this.RenderViewAsync("_CarPartial", vehicle, true);

                   
                    var categories = new List<string> { vehicle.City, vehicle.Category }; 

                    foreach (var category in categories.Distinct())
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveCarNotification", category, html);
                    }

                    TempData["SuccessMessage"] = "Vehicle added successfully!";
                    return RedirectToAction("Add");
                }

                TempData["ErrorMessage"] = "Failed to add vehicle!";
                return View(vehicle);
            }

            TempData["ErrorMessage"] = "Failed to add vehicle! Invalid data.";
            return View(vehicle);
        }
        private async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool isPartial)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

            var viewResult = isPartial
                ? _viewEngine.FindView(actionContext, viewName, false)
                : _viewEngine.GetView(null, viewName, false);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view '{viewName}'");
            }

            var viewDictionary = new ViewDataDictionary<TModel>(ViewData, model);

            using var sw = new StringWriter();

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }


        public IActionResult Update(string plateNumber)
        {
            return View(new Vehicle()); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _vehicleRepository.UpdateVehicle(vehicle);
                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Vehicle updated successfully!";
                    return RedirectToAction("Update"); 
                }
                TempData["ErrorMessage"] = "Vehicle update failed. Vehicle not found!";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data. Update failed.";
            }

            return View(vehicle); 
        }

      
        public IActionResult Delete(string plateNumber)
        {

            return View(new Vehicle()); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Vehicle vehicle)
        {
            bool isDeleted = _vehicleRepository.DeleteVehicle(vehicle);
            if (isDeleted)
            {
                TempData["SuccessMessage"] = "Vehicle deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Vehicle deletion failed. Vehicle not found!";
            }

            return RedirectToAction("Delete"); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAdmin(Vehicle vehicle)
        {
            bool isDeleted = _vehicleRepository.DeleteVehicle(vehicle);
            if (isDeleted)
            {
                TempData["ReturnMessage"] = "Vehicle deleted successfully!";
                TempData["ReturnSuccess"] = true;
                return RedirectToAction("AllVehicles"); // redirect here
            }

            TempData["ReturnMessage"] = "Vehicle deletion failed. Vehicle not found!";
            TempData["ReturnSuccess"] = false;
            return RedirectToAction("Details", new { id = vehicle.VehicleID }); // fallback
        }

        [HttpPost]
        public IActionResult ReturnVehicle(Vehicle vehicle)
        {
            bool result = _vehicleRepository.ReturnedVehicle(vehicle);

            if (result)
            {
                TempData["ReturnMessage"] = "Vehicle marked as returned.";
                TempData["ReturnSuccess"] = true;
            }
            else
            {
                TempData["ReturnMessage"] = "Vehicle not found or already returned.";
                TempData["ReturnSuccess"] = false;
            }

            return RedirectToAction("Details", new { id = vehicle.VehicleID });
        }



    }
}
