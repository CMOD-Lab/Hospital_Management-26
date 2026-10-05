using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for appointment-related business operations.
/// </summary>
public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets pending appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pending appointments for doctor: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets today's appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting today's appointments for doctor: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Approves an appointment.
    /// </summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment: {AppointmentId}", appointmentId);
            await _appointmentRepository.ApproveAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Deletes an appointment.
    /// </summary>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets current appointment for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            return appointment != null ? MapToDto(appointment) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets treatment history for a patient.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return appointments.Where(a => a.Status == "Completed").Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient: {PatientId}", patientId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets free slots for a doctor.
    /// </summary>
    public async Task<IEnumerable<int>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetFreeSlotsAsync(doctorId, patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<int>();
        }
    }

    /// <summary>
    /// Books an appointment.
    /// </summary>
    public async Task<bool> BookAppointmentAsync(AppointmentCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Booking appointment for patient: {PatientId} with doctor: {DoctorId}", dto.PatientId, dto.DoctorId);
            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                FreeSlot = dto.FreeSlot,
                Status = "Pending",
                AppointmentDate = DateTime.Today,
                CreatedDate = DateTime.UtcNow
            };
            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            return false;
        }
    }

    /// <summary>
    /// Gets notifications for a patient.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetNotificationsByPatientIdAsync(patientId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for patient: {PatientId}", patientId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets pending feedback appointment for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            return appointment != null ? MapToDto(appointment) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Stores feedback for an appointment.
    /// </summary>
    public async Task<bool> StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _appointmentRepository.StoreFeedbackAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Updates prescription for an appointment.
    /// </summary>
    public async Task<bool> UpdatePrescriptionAsync(PrescriptionUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            await _appointmentRepository.UpdatePrescriptionAsync(dto.DoctorId, dto.AppointmentId, dto.Disease, dto.Progress, dto.Prescription, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment: {AppointmentId}", dto.AppointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets patient history for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Where(a => a.Status == "Completed").Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient history for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    private static AppointmentDto MapToDto(Appointment a) => new()
    {
        AppointmentId = a.AppointmentId,
        DoctorId = a.DoctorId,
        DoctorName = a.Doctor?.Name ?? string.Empty,
        PatientId = a.PatientId,
        PatientName = a.Patient?.Name ?? string.Empty,
        FreeSlot = a.FreeSlot,
        Status = a.Status,
        AppointmentDate = a.AppointmentDate,
        Timings = a.Timings,
        Disease = a.Disease,
        Progress = a.Progress,
        Prescription = a.Prescription,
        FeedbackGiven = a.FeedbackGiven
    };
}
