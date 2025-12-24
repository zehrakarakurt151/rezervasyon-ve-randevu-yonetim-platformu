using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rezervasyonapi.DTOs;
using rezervasyonapi.Models;
using RezervasyonAPI.Data;
using RezervasyonAPI.DTOs;
using RezervasyonAPI.Services;

using System.Security.Claims;

namespace RezervasyonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AppointmentsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // 1. RANDEVU OLUŞTURMA (Müşteriler İçin)
        // POST: api/appointments
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int customerId = int.Parse(userIdString);

            // Müşteri bilgilerini al
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return Unauthorized();

            var service = await _context.Services.FindAsync(request.ServiceId);
            if (service == null) return BadRequest("Seçilen hizmet bulunamadı.");

            if (service.ServiceProviderId != request.ServiceProviderId)
                return BadRequest("Bu hizmet seçilen sağlayıcıya ait değil.");

            // Sağlayıcı bilgilerini al
            var provider = await _context.ServiceProviders.FindAsync(request.ServiceProviderId);
            if (provider == null) return BadRequest("Hizmet sağlayıcı bulunamadı.");

            // Saat Formatı Kontrolü
            if (!TimeSpan.TryParse(request.Time, out TimeSpan timeSpan))
            {
                return BadRequest("Saat formatı hatalı. (Örn: '09:00' olmalı)");
            }

            // Tarih ve Saati Birleştirme + UTC Ayarı
            DateTime combinedDate = request.Date.Date.Add(timeSpan);
            DateTime startTime = DateTime.SpecifyKind(combinedDate, DateTimeKind.Utc);

            DateTime endTime = startTime.AddMinutes(service.DurationMinutes);

            // ÇAKIŞMA KONTROLÜ
            bool isOccupied = await _context.Appointments.AnyAsync(a =>
                a.ServiceProviderId == request.ServiceProviderId &&
                a.Status != AppointmentStatus.CancelledByCustomer &&
                a.Status != AppointmentStatus.CancelledByProvider &&
                (startTime < a.EndTime && endTime > a.StartTime)
            );

            if (isOccupied)
            {
                return Conflict("Üzgünüz, seçtiğiniz saat aralığı az önce doldu.");
            }

            var newAppointment = new Appointment
            {
                CustomerId = customerId,
                ServiceProviderId = request.ServiceProviderId,
                ServiceId = request.ServiceId,
                StartTime = startTime,
                EndTime = endTime,
                TotalPrice = service.Price,
                Status = AppointmentStatus.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
                IsActive = true,
                CustomerNotes = "",
                ProviderCancellationReason = ""
            };

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            // E-posta bildirimleri gönder (arka planda, hata olsa bile devam et)
            _ = _emailService.SendAppointmentCreatedEmailAsync(
                customer.Email,
                provider.Email,
                service.Name,
                startTime,
                provider.BusinessName
            );

            return Ok(new { message = "Randevu oluşturuldu.", id = newAppointment.Id });
        }

        // In both GetMyAppointments and GetIncomingAppointments, convert DateTimeOffset to DateTime

        // 2. RANDEVULARIM (Müşteri İçin)
        [Authorize(Roles = "Customer")]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.ServiceProvider)
                .Where(a => a.CustomerId == userId)
                .OrderByDescending(a => a.StartTime)
                .Select(a => new AppointmentDetailDto
                {
                    Id = a.Id,
                    StartTime = a.StartTime.DateTime, // <-- FIX: Convert DateTimeOffset to DateTime
                    EndTime = a.EndTime.DateTime,     // <-- FIX: Convert DateTimeOffset to DateTime
                    ServiceName = a.Service.Name,
                    Price = a.TotalPrice,
                    Status = a.Status.ToString(),
                    ProviderName = a.ServiceProvider.BusinessName,
                    CustomerName = "Ben",
                    CustomerPhone = "-"
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // 3. GELEN RANDEVULAR (Dükkan Sahibi İçin)
        [Authorize(Roles = "ServiceProvider")]
        [HttpGet("incoming-appointments")]
        public async Task<IActionResult> GetIncomingAppointments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Customer)
                .Where(a => a.ServiceProviderId == userId)
                .OrderBy(a => a.StartTime)
                .Select(a => new AppointmentDetailDto
                {
                    Id = a.Id,
                    StartTime = a.StartTime.DateTime, // <-- FIX: Convert DateTimeOffset to DateTime
                    EndTime = a.EndTime.DateTime,     // <-- FIX: Convert DateTimeOffset to DateTime
                    ServiceName = a.Service.Name,
                    Price = a.TotalPrice,
                    Status = a.Status.ToString(),
                    ProviderName = "Ben",
                    CustomerName = a.Customer.FirstName + " " + a.Customer.LastName,
                    CustomerPhone = a.Customer.PhoneNumber
                })
                .ToListAsync();

            return Ok(appointments);
        }
        [Authorize]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Randevuyu ve ilişkili verileri bul
            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.ServiceProvider)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound("Randevu bulunamadı.");

            // GÜVENLİK: Başkasının randevusunu iptal edemesinler
            if (userRole == "Customer" && appointment.CustomerId != userId)
                return Unauthorized("Bu randevuyu iptal etme yetkiniz yok.");

            if (userRole == "ServiceProvider" && appointment.ServiceProviderId != userId)
                return Unauthorized("Bu randevuyu iptal etme yetkiniz yok.");

            // Zaten bitmiş veya iptal edilmişse işlem yapma
            if (appointment.Status == AppointmentStatus.Completed ||
                appointment.Status == AppointmentStatus.CancelledByCustomer ||
                appointment.Status == AppointmentStatus.CancelledByProvider)
            {
                return BadRequest("Bu randevu artık iptal edilemez.");
            }

            bool isCustomer = userRole == "Customer";
            
            // Statüyü güncelle
            if (isCustomer)
            {
                appointment.Status = AppointmentStatus.CancelledByCustomer;
                appointment.CustomerNotes = request.Reason;
            }
            else
            {
                appointment.Status = AppointmentStatus.CancelledByProvider;
                appointment.ProviderCancellationReason = request.Reason;
            }

            await _context.SaveChangesAsync();

            // E-posta bildirimleri gönder
            var emailToNotify = isCustomer ? appointment.ServiceProvider.Email : appointment.Customer.Email;
            _ = _emailService.SendAppointmentCancelledEmailAsync(
                emailToNotify,
                appointment.Service.Name,
                appointment.StartTime.DateTime,
                request.Reason ?? "",
                isCustomer
            );

            return Ok(new { message = "Randevu başarıyla iptal edildi." });
        }

        // 5. RANDEVU TAMAMLAMA (Sadece Dükkan Sahibi)
        // PUT: api/appointments/5/complete
        [Authorize(Roles = "ServiceProvider")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound("Randevu bulunamadı.");

            // GÜVENLİK: Sadece ilgili dükkan sahibi tamamlayabilir
            if (appointment.ServiceProviderId != userId)
                return Unauthorized("Bu işlem için yetkiniz yok.");

            // Durum kontrolü
            if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
            {
                return BadRequest("Sadece aktif randevular tamamlanabilir.");
            }

            appointment.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Hizmet tamamlandı olarak işaretlendi." });
        }
    }
}