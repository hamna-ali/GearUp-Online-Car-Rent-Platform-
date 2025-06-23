using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GearUp.Models;
using Microsoft.AspNetCore.Authorization;

namespace GearUp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    //[Authorize(Roles = "Admin")]
    [Authorize(Policy = "IsAdmin")]
    public IActionResult Admin_Index()
    {
        return View();
    }

    //[Authorize(Roles = "User")]
    [Authorize(Policy = "IsUser")]
    public IActionResult User_Index()
    {
        return View();
    }
    // GET: /Home/AboutUs
    [Authorize(Policy = "IsUser")]
    public IActionResult AboutUs()
    {
        return View();
    }

    // GET: /Home/ContactUs
    [Authorize(Policy = "IsUser")]
    public IActionResult ContactUs()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
