using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for patient-related business operations.
/// </summary>
public class PatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Validates login credentials and returns login result.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email: {Email}", loginDto.Email);
            var patient = await _patientRepository.GetByEmailAsync(loginDto.Email, cancellationToken);
            if (patient == null)
            {
                return new LoginResultDto { Status = 1, UserId = 0, UserType = 0 }; // Email not found
            }
            if (patient.Password != loginDto.Password)
            {
                return new LoginResultDto { Status = 2, UserId = 0, UserType = 0 }; // Wrong password
            }
            return new LoginResultDto { Status = 0, UserId = patient.PatientId, UserType = 1 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email: {Email}", loginDto.Email);
            return new LoginResultDto { Status = -1, UserId = 0, UserType = 0 };
        }
    }

    /// <summary>
    /// Registers a new patient.
    /// </summary>
    public async Task<(int status, int id)> SignupAsync(PatientCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Signing up patient with email: {Email}", dto.Email);
            var emailExists = await _patientRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                return (0, 0); // Email already exists
            }

            var patient = new Patient
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = DateTime.TryParse(dto.BirthDate, out var bd) ? bd : DateTime.Today,
                Gender = dto.Gender,
                Email = dto.Email,
                Password = dto.Password,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _patientRepository.AddAsync(patient, cancellationToken);
            _logger.LogInformation("Patient registered successfully with ID: {Id}", created.PatientId);
            return (1, created.PatientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing up patient with email: {Email}", dto.Email);
            return (-1, 0);
        }
    }

    /// <summary>
    /// Gets patient information by ID.
    /// </summary>
    public async Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting patient by ID: {Id}", id);
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
            if (patient == null) return null;

            return MapToDto(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient by ID: {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets all patients with optional search.
    /// </summary>
    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string? searchQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Patient> patients;
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);
            }
            else
            {
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            }
            return patients.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            return Enumerable.Empty<PatientDto>();
        }
    }

    private static PatientDto MapToDto(Patient patient) => new()
    {
        PatientId = patient.PatientId,
        Name = patient.Name,
        Phone = patient.Phone,
        Address = patient.Address,
        BirthDate = patient.BirthDate,
        Gender = patient.Gender,
        Email = patient.Email,
        IsActive = patient.IsActive
    };
}
