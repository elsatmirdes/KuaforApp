using KuaforApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace KuaforApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetDailyEarningsByEmployee/{id}")]
        public async Task<ActionResult<Values>> GetDailyEarningsByEmployee(int id)
        {
            DateTime selectedDate = DateTime.UtcNow.Date;

            var totalEarnings = await _context.Appointments
                .Where(a => a.EmployeeId == id && a.Date.Date == selectedDate)
                .SumAsync(a => (decimal?)a.Price) ?? 0;

            Values employeEarnings = await _context.Appointments
                .Where(x => x.UserId > 0) // userID değeri 0'dan büyük olanları filtrele
                .Select(x => new Values()
                {
                    employeeID = x.EmployeeId,
                    appointmentID = x.Id,
                    TotalEarnings = totalEarnings,
                    userID = x.UserId
                })
                .FirstOrDefaultAsync(x => x.employeeID == id);

            if (employeEarnings == null)
                return NotFound("İstenilen çalışan bulunamadı");

            return Ok(employeEarnings);
        }


        [HttpGet("GetTotalEarnings/{id}")]
        public async Task<ActionResult<Values>> GetTotalEarnings(int id)
        {
            var totalEarnings = await _context.Appointments
                .Where(a => a.EmployeeId == id)
                .SumAsync(a => (decimal?)a.Price) ?? 0;

            Values employeEarnings = await _context.Appointments
                .Where(x => x.UserId > 0) // userID değeri 0'dan büyük olanları filtrele
                .Select(x => new Values()
                {
                    employeeID = x.EmployeeId,
                    appointmentID = x.Id,
                    TotalEarnings = totalEarnings,
                    userID = x.UserId
                })
                .FirstOrDefaultAsync(x => x.employeeID == id);

            if (employeEarnings == null)
                return NotFound("İstenilen çalışan bulunamadı");

            return Ok(employeEarnings);
        }

    }
}
