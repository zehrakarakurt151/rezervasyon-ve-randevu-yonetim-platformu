using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rezervasyonapi.DTOs;
using rezervasyonapi.Models;
using RezervasyonAPI.Data;

using System.Security.Claims; // Kullanıcı ID'sini token'dan okumak için

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    // HİZMET EKLEME
    // Sadece giriş yapmış kullanıcılar erişebilir
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceDto request)
    {
        // 1. Token içinden kullanıcının ID'sini bul
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        int userId = int.Parse(userIdString);

        // 2. Bu kullanıcı gerçekten bir ServiceProvider mı?
        var provider = await _context.ServiceProviders.FindAsync(userId);
        if (provider == null)
        {
            return BadRequest("Sadece Hizmet Sağlayıcılar hizmet ekleyebilir.");
        }

        // 3. Hizmeti oluştur
        var newService = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            ServiceProviderId = userId, // İlişkiyi kuruyoruz
            CreatedDate = DateTimeOffset.UtcNow,
            IsActive = true
        };

        _context.Services.Add(newService);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Hizmet başarıyla eklendi.", serviceId = newService.Id });
    }

    // TÜM HİZMETLERİ LİSTELE (Herkes görebilir)
    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        var services = await _context.Services
            .Include(s => s.ServiceProvider) // Hangi dükkanın hizmeti olduğunu da getir
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Price,
                s.DurationMinutes,
                ProviderName = s.ServiceProvider.BusinessName
            })
            .ToListAsync();

        return Ok(services);
    }

    // KENDİ HİZMETLERİMİ GETİR (Provider için)
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyServices()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        int userId = int.Parse(userIdString);

        var services = await _context.Services
            .Where(s => s.ServiceProviderId == userId)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                s.Price,
                s.DurationMinutes,
                s.ServiceProviderId
            })
            .ToListAsync();

        return Ok(services);
    }

    // BELİRLİ SAĞLAYICININ HİZMETLERİ (Public)
    [HttpGet("provider/{providerId}")]
    public async Task<IActionResult> GetServicesByProvider(int providerId)
    {
        var services = await _context.Services
            .Where(s => s.ServiceProviderId == providerId)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                s.Price,
                s.DurationMinutes,
                s.ServiceProviderId
            })
            .ToListAsync();

        return Ok(services);
    }

    // HİZMET GÜNCELLE
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] CreateServiceDto request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        int userId = int.Parse(userIdString);

        var service = await _context.Services.FindAsync(id);
        if (service == null) return NotFound("Hizmet bulunamadı.");
        if (service.ServiceProviderId != userId) return Forbid();

        service.Name = request.Name;
        service.Description = request.Description;
        service.Price = request.Price;
        service.DurationMinutes = request.DurationMinutes;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Hizmet güncellendi." });
    }

    // HİZMET SİL
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        int userId = int.Parse(userIdString);

        var service = await _context.Services.FindAsync(id);
        if (service == null) return NotFound("Hizmet bulunamadı.");
        if (service.ServiceProviderId != userId) return Forbid();

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Hizmet silindi." });
    }
}