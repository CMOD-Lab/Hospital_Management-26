using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly BillService _billService;
    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();

    public BillsHistoryModel(BillService billService)
    {
        _billService = billService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("idoriginal");
        var userType = HttpContext.Session.GetInt32("userType");
        if (userId == null || userType != 1) return RedirectToPage("/Index");

        Bills = await _billService.GetBillHistoryAsync(userId.Value);
        return Page();
    }
}
