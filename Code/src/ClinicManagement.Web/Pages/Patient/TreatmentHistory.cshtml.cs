using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    public TreatmentHistoryModel(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        Appointments = await _appointmentService.GetTreatmentHistoryAsync(userId.Value);
        return Page();
    }
}
