using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for managing clinic staff, doctors, and patients.
/// </summary>
public class ManageClinicModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly DoctorService _doctorService;
    private readonly PatientService _patientService;
    private readonly ILogger<ManageClinicModel> _logger;

    public string Category { get; set; } = "DOCTOR";
    public string SearchQuery { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();
    public IEnumerable<PatientDto> Patients { get; set; } = new List<PatientDto>();
    public IEnumerable<StaffDto> Staff { get; set; } = new List<StaffDto>();

    public ManageClinicModel(AdminService adminService, DoctorService doctorService, PatientService patientService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _doctorService = doctorService;
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? category = "DOCTOR", string? search = "")
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");

        Category = category ?? "DOCTOR";
        SearchQuery = search ?? string.Empty;

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int id)
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");

        var success = await _doctorService.DeleteDoctorAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
            ? $"Doctor #{id} deleted successfully."
            : "Error deleting doctor.";

        return RedirectToPage(new { category = "DOCTOR" });
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int id)
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");

        var success = await _adminService.DeleteStaffAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
            ? $"Staff #{id} deleted successfully."
            : "Error deleting staff member.";

        return RedirectToPage(new { category = "OTHERSTAFF" });
    }

    private async Task LoadDataAsync()
    {
        switch (Category)
        {
            case "DOCTOR":
                Doctors = await _doctorService.GetAllDoctorsAsync(SearchQuery);
                break;
            case "PATIENT":
                Patients = await _patientService.GetAllPatientsAsync(SearchQuery);
                break;
            case "OTHERSTAFF":
                Staff = await _adminService.GetAllStaffAsync(SearchQuery);
                break;
        }
    }
}
