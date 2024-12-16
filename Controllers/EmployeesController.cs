using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KuaforApp.Models;

namespace KuaforApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            var salons = _context.Salons.ToList(); // Tüm salonları çekiyoruz
            var userList = _context.Users.Where(r => r.Role == "E").ToList();

            ViewBag.salonsName = new SelectList(salons, "Id", "Name"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.salonsId = new SelectList(salons, "Id", "Id"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.salonsNumber = new SelectList(salons, "Id", "ContactNumber"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.salonsAddress = new SelectList(salons, "Id", "Address"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.userID = new SelectList(userList, "Id", "Id");
            ViewBag.userName = new SelectList(userList, "Id", "FullName");

            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Specialty,IsAvailable,SalonId,Role,userID,start_available,finish_available")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            var salons = _context.Salons.ToList(); // Tüm salonları çekiyoruz
            var userList = _context.Users.Where(r => r.Role == "E").ToList();




            ViewBag.salonsName = new SelectList(salons, "Id", "Name"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.salonsId = new SelectList(salons, "Id", "Id"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.salonsNumber = new SelectList(salons, "Id", "ContactNumber"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.salonsAddress = new SelectList(salons, "Id", "Address"); // SelectList: Id = Value, Name = Gösterilecek Değer

            ViewBag.userID = new SelectList(userList, "Id", "Id");
            ViewBag.userName = new SelectList(userList, "Id", "FullName");

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["SalonId"] = new SelectList(_context.Salons, "Id", "Id", employee.SalonId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Specialty,IsAvailable,SalonId,finish_available,start_available")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employee.userID = _context.Employees.Where(e => e.Id == id).Select(e => e.userID).FirstOrDefault();
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SalonId"] = new SelectList(_context.Salons, "Id", "Id", employee.SalonId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
