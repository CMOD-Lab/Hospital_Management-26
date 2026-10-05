using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PreviousHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    public PreviousHistoryModel(AppointmentService appointmentService, ILogger<PreviousHistoryModel> logger)
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
