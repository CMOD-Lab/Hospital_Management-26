using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientNotificationsModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    public IEnumerable<AppointmentDto> Notifications { get; set; } = new List<AppointmentDto>();

    public PatientNotificationsModel(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        Notifications = await _appointmentService.GetNotificationsAsync(userId.Value);
        return Page();
    }
}
