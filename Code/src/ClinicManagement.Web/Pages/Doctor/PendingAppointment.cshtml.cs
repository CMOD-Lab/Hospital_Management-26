using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for pending appointments management.
/// </summary>
public class PendingAppointmentModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public PendingAppointmentModel(AppointmentService appointmentService, ILogger<PendingAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        Appointments = await _appointmentService.GetPendingAppointmentsAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        var success = await _appointmentService.ApproveAppointmentAsync(appointmentId);
        Message = success ? "Appointment approved successfully!" : "Error approving appointment.";
        IsSuccess = success;

        Appointments = await _appointmentService.GetPendingAppointmentsAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        var success = await _appointmentService.DeleteAppointmentAsync(appointmentId);
        Message = success ? "Appointment rejected successfully!" : "Error rejecting appointment.";
        IsSuccess = success;

        Appointments = await _appointmentService.GetPendingAppointmentsAsync(userId.Value);
        return Page();
    }
}
