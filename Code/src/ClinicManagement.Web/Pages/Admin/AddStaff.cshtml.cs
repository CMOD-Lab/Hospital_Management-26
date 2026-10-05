using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for adding staff members.
/// </summary>
public class AddStaffModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public AddStaffModel(AdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string birthDate, string phone, string gender,
        string address, int salary, string designation, string qualification)
    {
        var userType = HttpContext.Session.GetInt32("userType");
        if (userType != 3) return RedirectToPage("/Index");

        var dto = new StaffCreateDto
        {
            Name = name,
            BirthDate = birthDate,
            Phone = phone,
            Gender = gender,
            Address = address,
            Salary = salary,
            Designation = designation,
            Qualification = qualification
        };

        var success = await _adminService.AddStaffAsync(dto);
        if (success)
        {
            Message = "Staff member added successfully!";
            IsSuccess = true;
        }
        else
        {
            Message = "There was an error adding the staff member. Please try again.";
        }

        return Page();
    }
}
