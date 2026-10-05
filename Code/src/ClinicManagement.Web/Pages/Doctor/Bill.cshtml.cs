using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly BillService _billService;
    private readonly ILogger<BillModel> _logger;

    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public BillModel(BillService billService, ILogger<BillModel> logger)
    {
        _billService = billService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        Bills = await _billService.GetBillsByDoctorAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        var success = await _billService.MarkAsPaidAsync(userId.Value, appointmentId);
        Message = success ? "Bill marked as paid!" : "Error updating bill.";
        IsSuccess = success;

        Bills = await _billService.GetBillsByDoctorAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 2) return RedirectToPage("/Index");

        var success = await _billService.MarkAsUnpaidAsync(userId.Value, appointmentId);
        Message = success ? "Bill marked as unpaid!" : "Error updating bill.";
        IsSuccess = success;

        Bills = await _billService.GetBillsByDoctorAsync(userId.Value);
        return Page();
    }
}
