using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public int AppointmentId { get; set; }
    public AppointmentDto? Appointment { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public HistoryUpdateModel(AppointmentService appointmentService, ILogger<HistoryUpdateModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        AppointmentId = appointmentId;
        // Load appointment details
        var appointments = await _appointmentService.GetPatientHistoryAsync(userId.Value);
        Appointment = appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, string disease, string progress, string prescription)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        AppointmentId = appointmentId;

        var dto = new PrescriptionUpdateDto
        {
            DoctorId = userId.Value,
            AppointmentId = appointmentId,
            Disease = disease,
            Progress = progress,
            Prescription = prescription
        };

        var success = await _appointmentService.UpdatePrescriptionAsync(dto);
        Message = success ? "History updated successfully!" : "Error updating history.";
        IsSuccess = success;

        return Page();
    }
}
