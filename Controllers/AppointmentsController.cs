using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KuaforApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NuGet.Versioning;

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

            var appointments = _context.Appointments
            .Select(a => new Appointment
            {
                Id = a.Id,
                SalonId = a.SalonId,
                EmployeeId = a.EmployeeId,
                Service = a.Service,
                Date = a.Date.ToLocalTime(), // UTC -> Yerel zaman dönüşümü
                Time = a.Time,
                UserId = a.UserId,
                Price = a.Price
            })
            .ToList();


            return View(appointments);
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
            var salons = _context.Salons.ToList();


            // employee ıd sinden salon idsini alacağız

            ViewBag.employeesId = new SelectList(employees, "Id", "Id"); // SelectList: Id = Value, Name = Gösterilecek Değer
            // SelectList yerine serileştirilebilir bir listeye dönüştür
            ViewBag.employeesName = new SelectList(employees, "Id", "Name"); // SelectList: Id = Value, Name = Gösterilecek Değer
            ViewBag.specialty = new SelectList(employees, "Id", "Specialty"); // SelectList: Id = Value, Name = Gösterilecek Değer


            ViewBag.EmployeeName = _context.Employees.ToDictionary(e => e.Id, e => e.Name);
            ViewBag.EmployeeSalonID = _context.Employees.ToDictionary(e => e.Id, e => e.SalonId);
            ViewBag.SalonNames = _context.Salons.ToDictionary(s => s.Id, s => s.Name);


            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SalonId,EmployeeId,Service,Date,Time,Price")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {


                var appointment2 = await _context.Appointments
                .Select(a => new Appointment
                {
                    Id = a.Id,
                    SalonId = a.SalonId,
                    EmployeeId = a.EmployeeId,
                    Service = a.Service,
                    Date = a.Date.ToLocalTime(), // UTC -> Yerel zaman dönüşümü
                    Time = a.Time,
                    UserId = a.UserId,
                    Price = a.Price
                }).ToListAsync();


                var employees = _context.Employees.ToList();

               

                // EmployeeId'ye göre SalonId'yi bul
                appointment.SalonId = _context.Employees
                                               .Where(e => e.Id == appointment.EmployeeId)
                                               .Select(e => e.SalonId)
                                               .FirstOrDefault();

                // Çalışanın uygunluk bilgilerini kontrol et
                bool employeeIsAvailable = _context.Employees
                                                   .Where(e => e.Id == appointment.EmployeeId)
                                                   .Select(e => e.IsAvailable)
                                                   .FirstOrDefault();

                TimeOnly appointmentTime = TimeOnly.FromDateTime(appointment.Date.ToLocalTime()); // Randevu zamanı
                TimeOnly employeeStartTime = _context.Employees
                                                     .Where(e => e.Id == appointment.EmployeeId)
                                                     .Select(e => e.start_available)
                                                     .FirstOrDefault();
                TimeOnly employeeFinishTime = _context.Employees
                                                      .Where(e => e.Id == appointment.EmployeeId)
                                                      .Select(e => e.finish_available)
                                                      .FirstOrDefault();

                // Yeni randevunun başlangıç ve bitiş saatlerini belirleyin
                TimeOnly appointmentStartTime = TimeOnly.FromDateTime(appointment.Date);
                TimeOnly appointmentEndTime = appointmentStartTime.Add(appointment.Time);

                // Hataları ve formu tekrar göster
                ViewBag.employeesId = new SelectList(employees, "Id", "Id"); // SelectList: Id = Value, Name = Gösterilecek Değer
                                                                             // SelectList yerine serileştirilebilir bir listeye dönüştür
                ViewBag.employeesName = new SelectList(employees, "Id", "Name"); // SelectList: Id = Value, Name = Gösterilecek Değer
                ViewBag.specialty = new SelectList(employees, "Id", "Specialty"); // SelectList: Id = Value, Name = Gösterilecek Değer

                ViewBag.EmployeeName = _context.Employees.ToDictionary(e => e.Id, e => e.Name);
                ViewBag.EmployeeSalonID = _context.Employees.ToDictionary(e => e.Id, e => e.SalonId);
                ViewBag.SalonNames = _context.Salons.ToDictionary(s => s.Id, s => s.Name);

                // Çalışanın mevcut randevularını alın
                var existingAppointments = appointment2
                    .Where(a => a.EmployeeId == appointment.EmployeeId && a.Date.Date == appointment.Date.Date) // Aynı gün içinde
                    .ToList();

                // Çakışmayı kontrol edin
                bool isConflict = existingAppointments.Any(a =>
                {
                    TimeOnly existingStartTime = TimeOnly.FromDateTime(a.Date);
                    TimeOnly existingEndTime = existingStartTime.Add(a.Time);

                    TempData["addAppointmetStartTime"] = appointmentStartTime.ToString();
                    TempData["dbAppointmetExistEndTime"] = existingEndTime.ToString();
                    TempData["addAppointmetEndTime"] = appointmentEndTime.ToString();
                    TempData["dbAppointmetExistStartTime"] = existingStartTime.ToString();

                    return (appointmentStartTime > existingStartTime && appointmentStartTime < existingEndTime)|| (appointmentEndTime > existingStartTime && appointmentEndTime < existingEndTime);
                });


                if (isConflict)
                {
                    ModelState.AddModelError("", "Seçtiğiniz saat dilimi, çalışanın başka bir randevusu ile çakışıyor.");



                    return View(appointment);
                }

                // Çalışma saatleri içinde mi kontrol et
                if (!(employeeIsAvailable && employeeStartTime <= appointmentStartTime && employeeFinishTime >= appointmentEndTime))
                {

                    if (!employeeIsAvailable)
                    {
                        ModelState.AddModelError("", $"Seçtiğiniz çalışan, randevu için uygun değil.");

                    }
                    else
                    {
                        ModelState.AddModelError("", $"Seçtiğiniz çalışan, {appointment.Date:HH:mm} saatleri arasında uygun değil.");
                    }
                    return View(appointment);
                }

                // Randevuyu kaydet
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //// Eğer ModelState geçerli değilse, formu tekrar göster
            var employeesList = _context.Employees.ToList();
            //ViewBag.EmployeeName = _context.Employees.ToDictionary(e => e.Id, e => e.Name);
            //ViewBag.EmployeeSalonID = _context.Employees.ToDictionary(e => e.Id, e => e.SalonId);
            //ViewBag.SalonNames = _context.Salons.ToDictionary(s => s.Id, s => s.Name);

            ViewBag.employeesId = new SelectList(employeesList, "Id", "Id");
            ViewBag.specialty = new SelectList(employeesList, "Id", "Specialty");

            return View(appointment);
        }

        //

        [Authorize(Roles = "U")]
        public IActionResult userAppointments()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));

            ViewBag.userid = Convert.ToInt32(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            ViewBag.SalonNames = _context.Salons.ToDictionary(s => s.Id, s => s.Name);
            ViewBag.EmployeeName = _context.Employees.ToDictionary(e => e.Id, e => e.Name);
            ViewBag.userAppointments = _context.Appointments
            .Where(a => a.UserId == userID)
            .Select(a => new Appointment
            {
                Id = a.Id,
                SalonId = a.SalonId,
                EmployeeId = a.EmployeeId,
                Service = a.Service,
                Date = a.Date.ToLocalTime(), // Tarihi yerel zamana dönüştür
                Time = a.Time,
                Price = a.Price,
                acceptAppointment = a.acceptAppointment,
                UserId = a.UserId
            })
            .ToList();



            return View();
        }

        [Authorize(Roles = "E")]
        public IActionResult EmployeeAppointments()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            var employeeID = Convert.ToInt32(_context.Employees.Where(e => e.userID == userID).Select(u => u.Id).FirstOrDefault());



            ViewBag.employeeAppointments = _context.Appointments
            .Where(a => a.EmployeeId == employeeID)
            .Select(a => new Appointment
            {
                Id = a.Id,
                SalonId = a.SalonId,
                EmployeeId = a.EmployeeId,
                Service = a.Service,
                Date = a.Date.ToLocalTime(), // Tarihi yerel zamana dönüştür
                Time = a.Time,
                Price = a.Price,
                acceptAppointment = a.acceptAppointment,
                UserId = a.UserId
            })
            .ToList();

            ViewBag.userName = _context.Users.ToDictionary(u => u.Id, u => u.FullName);

            return View();
        }

        public async Task<IActionResult> acces_appointment(int id, [Bind("Id,SalonId,EmployeeId,Service,Date,Time,Price,acceptAppointment,UserId")] Appointment appointment)
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
                    return NotFound();

                }
                return View();
            }
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
            .Select(a => new Appointment
            {
                Id = a.Id,
                SalonId = a.SalonId,
                EmployeeId = a.EmployeeId,
                Service = a.Service,
                Date = a.Date.ToLocalTime(), // UTC -> Yerel zaman dönüşümü
                Time = a.Time,
                UserId = a.UserId,
                Price = a.Price
            })
            .Where(a => a.Id == id).FirstOrDefaultAsync();

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

        public bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
