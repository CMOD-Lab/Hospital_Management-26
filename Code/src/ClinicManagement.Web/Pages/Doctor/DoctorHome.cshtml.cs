using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for the doctor home page.
/// </summary>
public class DoctorHomeModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public DoctorDto? Doctor { get; set; }

    public DoctorHomeModel(DoctorService doctorService, ILogger<DoctorHomeModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        Doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
        if (Doctor == null)
        {
            _logger.LogWarning("Doctor not found for ID: {Id}", userId);
        }

        return Page();
    }
}
