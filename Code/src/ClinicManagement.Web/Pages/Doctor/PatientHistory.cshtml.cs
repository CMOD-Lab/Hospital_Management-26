using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    public PatientHistoryModel(AppointmentService appointmentService, ILogger<PatientHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        Appointments = await _appointmentService.GetPatientHistoryAsync(userId.Value);
        return Page();
    }
}
