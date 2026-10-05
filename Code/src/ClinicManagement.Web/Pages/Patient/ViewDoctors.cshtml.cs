using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class ViewDoctorsModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly AdminService _adminService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public string SelectedDept { get; set; } = string.Empty;

    public ViewDoctorsModel(DoctorService doctorService, AdminService adminService, ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? deptName = "")
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        SelectedDept = deptName ?? string.Empty;
        Departments = await _adminService.GetAllDepartmentsAsync();

        if (!string.IsNullOrWhiteSpace(SelectedDept))
        {
            Doctors = await _doctorService.GetDoctorsByDepartmentAsync(SelectedDept);
        }
        else
        {
            Doctors = await _doctorService.GetAllDoctorsAsync();
        }

        return Page();
    }
}
