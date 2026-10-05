using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for admin-related business operations.
/// </summary>
public class AdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IBillRepository _billRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IDepartmentRepository departmentRepository,
        IBillRepository billRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _departmentRepository = departmentRepository;
        _billRepository = billRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets admin dashboard data.
    /// </summary>
    public async Task<AdminDashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting admin dashboard data");
            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            var bills = await _billRepository.GetAllAsync(cancellationToken);

            var totalIncome = bills.Where(b => b.IsPaid).Sum(b => b.Amount);

            return new AdminDashboardDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                Departments = departments.Select(d => new DepartmentDto
                {
                    DeptNo = d.DeptNo,
                    DeptName = d.DeptName,
                    Description = d.Description,
                    DoctorCount = d.Doctors.Count
                }),
                RecentAppointments = appointments.Take(10).Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor?.Name ?? string.Empty,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.Name ?? string.Empty,
                    Status = a.Status,
                    AppointmentDate = a.AppointmentDate
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin dashboard data");
            return new AdminDashboardDto();
        }
    }

    /// <summary>
    /// Adds a new staff member.
    /// </summary>
    public async Task<bool> AddStaffAsync(StaffCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding staff member: {Name}", dto.Name);
            var staff = new OtherStaff
            {
                Name = dto.Name,
                BirthDate = DateTime.TryParse(dto.BirthDate, out var bd) ? bd : DateTime.Today,
                Phone = dto.Phone,
                Gender = dto.Gender,
                Address = dto.Address,
                Salary = dto.Salary,
                Designation = dto.Designation,
                Qualification = dto.Qualification,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            await _staffRepository.AddAsync(staff, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member");
            return false;
        }
    }

    /// <summary>
    /// Deletes a staff member.
    /// </summary>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff member with ID: {Id}", id);
            await _staffRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member with ID: {Id}", id);
            return false;
        }
    }

    /// <summary>
    /// Gets all staff members with optional search.
    /// </summary>
    public async Task<IEnumerable<StaffDto>> GetAllStaffAsync(string? searchQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<OtherStaff> staff;
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);
            }
            else
            {
                staff = await _staffRepository.GetAllAsync(cancellationToken);
            }
            return staff.Select(s => new StaffDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Phone = s.Phone,
                Gender = s.Gender,
                Address = s.Address,
                Salary = s.Salary,
                Designation = s.Designation,
                Qualification = s.Qualification,
                IsActive = s.IsActive,
                BirthDate = s.BirthDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all staff");
            return Enumerable.Empty<StaffDto>();
        }
    }

    /// <summary>
    /// Gets all departments.
    /// </summary>
    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return departments.Select(d => new DepartmentDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description,
                DoctorCount = d.Doctors.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }
}
