using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RezervasyonAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class ProvidersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProvidersController(AppDbContext context)
    {
        _context = context;
    }

    // TÜM HİZMET SAĞLAYICILARI LİSTELE (Herkes görebilir)
    [HttpGet]
    public async Task<IActionResult> GetAllProviders()
    {
        var providers = await _context.ServiceProviders
            .Select(p => new
            {
                p.Id,
                p.BusinessName,
                p.FirstName,
                p.LastName,
                p.Address,
                p.Description,
                p.AverageRating,
                ServiceCount = p.Services.Count
            })
            .ToListAsync();

        return Ok(providers);
    }

    // TEK SAĞLAYICININ DETAYLARI
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProviderById(int id)
    {
        var provider = await _context.ServiceProviders
            .Include(p => p.Services.Where(s => s.IsActive))
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.BusinessName,
                p.FirstName,
                p.LastName,
                p.Address,
                p.Description,
                p.AverageRating,
                Services = p.Services.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Price,
                    s.DurationMinutes
                })
            })
            .FirstOrDefaultAsync();

        if (provider == null)
            return NotFound("Hizmet sağlayıcı bulunamadı.");

        return Ok(provider);
    }

    // SAĞLAYICININ HİZMETLERİ
    [HttpGet("{id}/services")]
    public async Task<IActionResult> GetProviderServices(int id)
    {
        var services = await _context.Services
            .Where(s => s.ServiceProviderId == id && s.IsActive)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                s.Price,
                s.DurationMinutes
            })
            .ToListAsync();

        return Ok(services);
    }
}
