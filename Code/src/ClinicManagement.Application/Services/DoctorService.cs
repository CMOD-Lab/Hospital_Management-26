using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for doctor-related business operations.
/// </summary>
public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDoctorRepository doctorRepository, IDepartmentRepository departmentRepository, ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Validates doctor login credentials.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating doctor login for email: {Email}", loginDto.Email);
            var doctor = await _doctorRepository.GetByEmailAsync(loginDto.Email, cancellationToken);
            if (doctor == null)
            {
                return new LoginResultDto { Status = 1, UserId = 0, UserType = 0 };
            }
            if (doctor.Password != loginDto.Password)
            {
                return new LoginResultDto { Status = 2, UserId = 0, UserType = 0 };
            }
            return new LoginResultDto { Status = 0, UserId = doctor.DoctorId, UserType = 2 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating doctor login");
            return new LoginResultDto { Status = -1, UserId = 0, UserType = 0 };
        }
    }

    /// <summary>
    /// Checks if a doctor email already exists.
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor email existence: {Email}", email);
            return false;
        }
    }

    /// <summary>
    /// Adds a new doctor.
    /// </summary>
    public async Task<bool> AddDoctorAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding doctor with email: {Email}", dto.Email);
            var doctor = new Doctor
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                BirthDate = DateTime.TryParse(dto.BirthDate, out var bd) ? bd : DateTime.Today,
                DeptNo = dto.DeptNo,
                Gender = dto.Gender,
                Address = dto.Address,
                Experience = dto.Experience,
                Salary = dto.Salary,
                ChargesPerVisit = dto.ChargesPerVisit,
                Phone = dto.Phone,
                Specialization = dto.Specialization,
                Qualification = dto.Qualification,
                Status = true,
                CreatedDate = DateTime.UtcNow
            };
            await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor added successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor");
            return false;
        }
    }

    /// <summary>
    /// Deletes (deactivates) a doctor by ID.
    /// </summary>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting doctor with ID: {Id}", id);
            await _doctorRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {Id}", id);
            return false;
        }
    }

    /// <summary>
    /// Gets all doctors with optional search.
    /// </summary>
    public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? searchQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Doctor> doctors;
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
            }
            else
            {
                doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            }
            return doctors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all doctors");
            return Enumerable.Empty<DoctorDto>();
        }
    }

    /// <summary>
    /// Gets a doctor by ID.
    /// </summary>
    public async Task<DoctorDto?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);
            if (doctor == null) return null;
            return MapToDto(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor by ID: {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets doctors by department name.
    /// </summary>
    public async Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);
            return doctors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors by department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorDto>();
        }
    }

    private static DoctorDto MapToDto(Doctor doctor) => new()
    {
        DoctorId = doctor.DoctorId,
        Name = doctor.Name,
        Email = doctor.Email,
        Phone = doctor.Phone,
        Gender = doctor.Gender,
        Address = doctor.Address,
        DeptNo = doctor.DeptNo,
        DeptName = doctor.Department?.DeptName ?? string.Empty,
        Experience = doctor.Experience,
        Salary = doctor.Salary,
        ChargesPerVisit = doctor.ChargesPerVisit,
        Specialization = doctor.Specialization,
        Qualification = doctor.Qualification,
        ReputeIndex = doctor.ReputeIndex,
        PatientsTreated = doctor.PatientsTreated,
        Status = doctor.Status,
        BirthDate = doctor.BirthDate
    };
}
