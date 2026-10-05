using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>
/// Page model for the main login/signup page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;
    private readonly ILogger<IndexModel> _logger;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public IndexModel(PatientService patientService, DoctorService doctorService, ILogger<IndexModel> logger)
    {
        _patientService = patientService;
        _doctorService = doctorService;
        _logger = logger;
    }

    public void OnGet()
    {
        // Clear session on landing page
        HttpContext.Session.Clear();
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    public async Task<IActionResult> OnPostLoginAsync(string loginEmail, string loginPassword)
    {
        if (string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword))
        {
            ErrorMessage = "Please enter email and password.";
            return Page();
        }

        var loginDto = new LoginDto { Email = loginEmail, Password = loginPassword };

        // Try patient login first
        var patientResult = await _patientService.ValidateLoginAsync(loginDto);
        if (patientResult.Status == 0)
        {
            HttpContext.Session.SetInt32("idoriginal", patientResult.UserId);
            HttpContext.Session.SetInt32("userType", 1);
            _logger.LogInformation("Patient logged in: {UserId}", patientResult.UserId);
            return RedirectToPage("/Patient/PatientHome");
        }

        // Try doctor login
        var doctorResult = await _doctorService.ValidateLoginAsync(loginDto);
        if (doctorResult.Status == 0)
        {
            HttpContext.Session.SetInt32("idoriginal", doctorResult.UserId);
            HttpContext.Session.SetInt32("userType", 2);
            _logger.LogInformation("Doctor logged in: {UserId}", doctorResult.UserId);
            return RedirectToPage("/Doctor/DoctorHome");
        }

        // Admin login (dbo.logintable type = 3)
        var adminLogin = await _patientService.ValidateAdminLoginAsync(loginDto);
        if (adminLogin.Status == 0)
        {
            HttpContext.Session.SetInt32("idoriginal", adminLogin.UserId);
            HttpContext.Session.SetInt32("userType", 3);
            _logger.LogInformation("Admin logged in");
            return RedirectToPage("/Admin/AdminHome");
        }

        // Determine error message
        if (patientResult.Status == 1 && doctorResult.Status == 1)
        {
            ErrorMessage = "Email not found. Please try again!";
        }
        else if (patientResult.Status == 2 || doctorResult.Status == 2)
        {
            ErrorMessage = "Incorrect password. Please try again!";
        }
        else
        {
            ErrorMessage = "There was an error. Please try again!";
        }

        return Page();
    }

    /// <summary>
    /// Handles patient signup form submission.
    /// </summary>
    public async Task<IActionResult> OnPostSignupAsync(
        string signupName, string signupBirthDate, string signupEmail,
        string signupPassword, string signupPhone, string signupGender, string signupAddress)
    {
        if (string.IsNullOrWhiteSpace(signupName) || string.IsNullOrWhiteSpace(signupEmail) ||
            string.IsNullOrWhiteSpace(signupPassword))
        {
            ErrorMessage = "Please fill in all required fields.";
            return Page();
        }

        var dto = new PatientCreateDto
        {
            Name = signupName,
            BirthDate = signupBirthDate,
            Email = signupEmail,
            Password = signupPassword,
            Phone = signupPhone,
            Gender = signupGender,
            Address = signupAddress
        };

        var (status, id) = await _patientService.SignupAsync(dto);

        if (status == 0)
        {
            ErrorMessage = "Email already exists. Please choose a different one.";
            return Page();
        }
        else if (status == 1)
        {
            HttpContext.Session.SetInt32("idoriginal", id);
            HttpContext.Session.SetInt32("userType", 1);
            _logger.LogInformation("New patient registered: {Id}", id);
            return RedirectToPage("/Patient/PatientHome");
        }
        else
        {
            ErrorMessage = "There was an error during registration. Please try again!";
            return Page();
        }
    }
}
