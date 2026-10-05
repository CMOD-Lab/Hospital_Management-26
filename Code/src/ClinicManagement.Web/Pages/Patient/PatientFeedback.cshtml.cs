using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    public AppointmentDto? PendingFeedback { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public PatientFeedbackModel(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        PendingFeedback = await _appointmentService.GetPendingFeedbackAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        var success = await _appointmentService.StoreFeedbackAsync(appointmentId);
        Message = success ? "Thank you for your feedback!" : "Error submitting feedback.";
        IsSuccess = success;

        if (!success)
        {
            PendingFeedback = await _appointmentService.GetPendingFeedbackAsync(userId.Value);
        }

        return Page();
    }
}
