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
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {

            ViewBag.SalonNames = _context.Salons.ToDictionary(s => s.Id, s => s.Name);
            ViewBag.EmployeeName = _context.Employees.ToDictionary(e => e.Id, e => e.Name);
            return View(await _context.Appointments.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            var employees = _context.Employees.ToList();

            // employee ıd sinden salon idsini alacağız

            ViewBag.EmployeeName = _context.Employees.ToDictionary(e => e.Id, e => e.Name);

            ViewBag.EmployeeSalonID = _context.Employees.ToDictionary(e => e.Id, e => e.SalonId);
            ViewBag.SalonNames = _context.Salons.ToDictionary(s => s.Id, s => s.Name);


            ViewBag.employeesId = new SelectList(employees, "Id", "Id"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.employeesName = new SelectList(employees, "Id", "Name"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.specialty = new SelectList(employees, "Id", "Specialty"); // SelectList: Id = Value, Name = Gösterilecek Değer

            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SalonId,EmployeeId,Service,Date,Time,Price,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // EmployeeId'ye göre SalonId'yi bul
                appointment.SalonId = _context.Employees
                                               .Where(e => e.Id == appointment.EmployeeId)
                                               .Select(e => e.SalonId)
                                               .FirstOrDefault();



                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var employees = _context.Employees.ToList();
            ViewBag.employee = employees;

           
            ViewBag.employeesId = new SelectList(employees, "Id", "Id"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.specialty = new SelectList(employees, "Id", "Specialty"); // SelectList: Id = Value, Name = Gösterilecek Değer


            return View(appointment);
        }



        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SalonId,EmployeeId,Service,Date,Time,Price,UserId")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
