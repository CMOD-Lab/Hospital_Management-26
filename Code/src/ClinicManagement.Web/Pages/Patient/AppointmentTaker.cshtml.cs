using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentTakerModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly DoctorService _doctorService;
    private readonly ILogger<AppointmentTakerModel> _logger;

    public DoctorDto? Doctor { get; set; }
    public IEnumerable<int> FreeSlots { get; set; } = new List<int>();
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public AppointmentTakerModel(AppointmentService appointmentService, DoctorService doctorService, ILogger<AppointmentTakerModel> logger)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        Doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int freeSlot)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        var dto = new AppointmentCreateDto
        {
            DoctorId = doctorId,
            PatientId = userId.Value,
            FreeSlot = freeSlot
        };

        var success = await _appointmentService.BookAppointmentAsync(dto);
        if (success)
        {
            return RedirectToPage("/Patient/AppointmentRequestSent");
        }

        Message = "Error booking appointment. Please try again.";
        Doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, userId.Value);
        return Page();
    }
}
