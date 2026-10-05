using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorDto? Doctor { get; set; }

    public DoctorProfileModel(DoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        Doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
        return Page();
    }
}
