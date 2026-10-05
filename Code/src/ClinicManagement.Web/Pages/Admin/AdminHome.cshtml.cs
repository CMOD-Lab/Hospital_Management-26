using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin home dashboard.
/// </summary>
public class AdminHomeModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminDashboardDto Dashboard { get; set; } = new();

    public AdminHomeModel(AdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 3)
        {
            return RedirectToPage("/Index");
        }

        try
        {
            Dashboard = await _adminService.GetDashboardDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            TempData["ErrorMessage"] = "Error loading dashboard data.";
        }

        return Page();
    }
}
