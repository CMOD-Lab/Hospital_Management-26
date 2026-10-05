using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientHomeModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientDto? Patient { get; set; }
    public AppointmentDto? CurrentAppointment { get; set; }

    public PatientHomeModel(PatientService patientService, AppointmentService appointmentService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        Patient = await _patientService.GetPatientByIdAsync(userId.Value);
        CurrentAppointment = await _appointmentService.GetCurrentAppointmentAsync(userId.Value);

        return Page();
    }
}
