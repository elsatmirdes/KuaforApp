using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "A")]
public class AdminController : Controller
{

    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new
        {
            TotalUsers = _context.Users.Count(),
            TotalEmployees = _context.Employees.Count(),
            TotalAppointments = _context.Appointments.Count(),
            TotalSalons = _context.Salons.Count()
        };
        return View(model);
    }

    public IActionResult Users()
    {
        var users = _context.Users.ToList();
        return RedirectToAction("Index","Users");
    }

    public IActionResult Employees()
    {
        var employees = _context.Employees.ToList();
        return RedirectToAction("Index", "Employees");
    }

    public IActionResult Appointments()
    {
        var appointments = _context.Appointments.ToList();
        return RedirectToAction("Index", "Appointments");
    }

    public IActionResult Salons()
    {
        var salons = _context.Salons.ToList();
        return RedirectToAction("Index", "Salons");
    }

    [HttpGet]
    [Authorize]
    public IActionResult AccessDenied()
    {
        return RedirectToAction("Login", "AccessDenied");
    }
}