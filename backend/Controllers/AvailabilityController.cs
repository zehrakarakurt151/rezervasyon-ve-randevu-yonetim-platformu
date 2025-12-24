using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rezervasyonapi.Models;
using RezervasyonAPI.Data;


namespace RezervasyonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AvailabilityController(AppDbContext context)
        {
            _context = context;
        }
        // tarihleri gireken 2025-11-25 gibi girmeniz lazım api de
        [HttpGet("slots")]
        public async Task<IActionResult> GetAvailableSlots(int providerId, int serviceId, DateTime date)
        {
            var schedule = await _context.WorkSchedules
                .FirstOrDefaultAsync(w => w.ServiceProviderId == providerId && w.DayOfWeek == date.DayOfWeek);

            // Varsayılan çalışma saatleri (schedule yoksa veya çalışmıyorsa)
            TimeSpan startTime = TimeSpan.FromHours(9);  // 09:00
            TimeSpan endTime = TimeSpan.FromHours(18);   // 18:00
            
            if (schedule != null)
            {
                if (!schedule.IsWorking)
                {
                    return Ok(new List<string>()); // Bu gün çalışmıyor
                }
                startTime = schedule.StartTime;
                endTime = schedule.EndTime;
            }

            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return BadRequest("Hizmet bulunamadı.");

            int duration = service.DurationMinutes;

            var existingAppointments = await _context.Appointments
                .Where(a => a.ServiceProviderId == providerId
                         && a.StartTime.Date == date.Date
                         && a.Status != AppointmentStatus.CancelledByCustomer
                         && a.Status != AppointmentStatus.CancelledByProvider)
                .ToListAsync();

            List<string> availableSlots = new List<string>();

            TimeSpan currentSlotStart = startTime;

            while (currentSlotStart.Add(TimeSpan.FromMinutes(duration)) <= endTime)
            {
                TimeSpan currentSlotEnd = currentSlotStart.Add(TimeSpan.FromMinutes(duration));

                DateTime slotStartDateTime = date.Date.Add(currentSlotStart);
                DateTime slotEndDateTime = date.Date.Add(currentSlotEnd);

                bool isTaken = existingAppointments.Any(app =>
                    (slotStartDateTime < app.EndTime.DateTime) && (slotEndDateTime > app.StartTime.DateTime));

                if (!isTaken)
                {
                    availableSlots.Add(currentSlotStart.ToString(@"hh\:mm"));
                }

                currentSlotStart = currentSlotStart.Add(TimeSpan.FromMinutes(duration));
            }

            return Ok(availableSlots);
        }
    }
}