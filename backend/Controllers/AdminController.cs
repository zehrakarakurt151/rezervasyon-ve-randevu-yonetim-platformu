using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rezervasyonapi.Models;
using RezervasyonAPI.Data;

namespace RezervasyonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard istatistikleri
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var totalProviders = await _context.ServiceProviders.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();
            var pendingAppointments = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Pending)
                .CountAsync();
            var completedAppointments = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .CountAsync();
            var totalRevenue = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .SumAsync(a => a.TotalPrice);

            return Ok(new
            {
                totalCustomers,
                totalProviders,
                totalAppointments,
                pendingAppointments,
                completedAppointments,
                totalRevenue
            });
        }

        // Tüm kullanıcıları listele
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    Role = u.Role.ToString(),
                    u.CreatedDate
                })
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();

            return Ok(users);
        }

        // Tüm randevuları listele
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.ServiceProvider)
                .Include(a => a.Service)
                .Select(a => new
                {
                    a.Id,
                    CustomerName = a.Customer.FirstName + " " + a.Customer.LastName,
                    ProviderName = a.ServiceProvider.BusinessName,
                    ServiceName = a.Service.Name,
                    a.StartTime,
                    a.EndTime,
                    a.TotalPrice,
                    Status = a.Status.ToString(),
                    a.CreatedDate
                })
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();

            return Ok(appointments);
        }

        // Tüm hizmetleri listele
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _context.Services
                .Include(s => s.ServiceProvider)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Price,
                    s.DurationMinutes,
                    ProviderName = s.ServiceProvider.BusinessName,
                    s.IsActive
                })
                .ToListAsync();

            return Ok(services);
        }
    }
}
