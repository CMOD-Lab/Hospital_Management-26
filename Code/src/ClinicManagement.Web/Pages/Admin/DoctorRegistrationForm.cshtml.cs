using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for doctor registration form.
/// </summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly AdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string EmailError { get; set; } = string.Empty;
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public DoctorCreateDto Input { get; set; } = new();

    public DoctorRegistrationFormModel(DoctorService doctorService, AdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
    {
        _doctorService = doctorService;
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");

        Departments = await _adminService.GetAllDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string email, string password, string birthDate,
        int deptNo, string gender, string phone, string address,
        int experience, int salary, int chargesPerVisit, string specialization, string qualification)
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");

        Departments = await _adminService.GetAllDepartmentsAsync();

        // Check email uniqueness
        if (await _doctorService.EmailExistsAsync(email))
        {
            EmailError = "This email already exists. Please choose a different one!";
            Input = new DoctorCreateDto { Name = name, Email = email, BirthDate = birthDate, DeptNo = deptNo, Gender = gender, Phone = phone, Address = address, Experience = experience, Salary = salary, ChargesPerVisit = chargesPerVisit, Specialization = specialization, Qualification = qualification };
            return Page();
        }

        if (deptNo == 0)
        {
            Message = "Please select a department.";
            Input = new DoctorCreateDto { Name = name, Email = email, BirthDate = birthDate, DeptNo = deptNo, Gender = gender, Phone = phone, Address = address, Experience = experience, Salary = salary, ChargesPerVisit = chargesPerVisit, Specialization = specialization, Qualification = qualification };
            return Page();
        }

        var dto = new DoctorCreateDto
        {
            Name = name,
            Email = email,
            Password = password,
            BirthDate = birthDate,
            DeptNo = deptNo,
            Gender = gender,
            Phone = phone,
            Address = address,
            Experience = experience,
            Salary = salary,
            ChargesPerVisit = chargesPerVisit,
            Specialization = specialization,
            Qualification = qualification
        };

        var success = await _doctorService.AddDoctorAsync(dto);
        if (success)
        {
            Message = "Doctor added successfully!";
            IsSuccess = true;
            Input = new DoctorCreateDto();
        }
        else
        {
            Message = "There was an error adding the doctor. Please try again.";
        }

        return Page();
    }
}
