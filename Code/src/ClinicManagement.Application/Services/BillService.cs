using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for bill-related business operations.
/// </summary>
public class BillService
{
    private readonly IBillRepository _billRepository;
    private readonly ILogger<BillService> _logger;

    public BillService(IBillRepository billRepository, ILogger<BillService> logger)
    {
        _billRepository = billRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets bill history for a patient.
    /// </summary>
    public async Task<IEnumerable<BillDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bill history for patient: {PatientId}", patientId);
            var bills = await _billRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return bills.Select(b => new BillDto
            {
                BillId = b.BillId,
                PatientId = b.PatientId,
                PatientName = b.Patient?.Name ?? string.Empty,
                AppointmentId = b.AppointmentId,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                BillDate = b.BillDate,
                Description = b.Description,
                DoctorName = b.Appointment?.Doctor?.Name ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient: {PatientId}", patientId);
            return Enumerable.Empty<BillDto>();
        }
    }

    /// <summary>
    /// Gets bills for a doctor to generate.
    /// </summary>
    public async Task<IEnumerable<BillDto>> GetBillsByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bills for doctor: {DoctorId}", doctorId);
            var bills = await _billRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return bills.Select(b => new BillDto
            {
                BillId = b.BillId,
                PatientId = b.PatientId,
                PatientName = b.Patient?.Name ?? string.Empty,
                AppointmentId = b.AppointmentId,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                BillDate = b.BillDate,
                Description = b.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bills for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<BillDto>();
        }
    }

    /// <summary>
    /// Marks a bill as paid.
    /// </summary>
    public async Task<bool> MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as paid for appointment: {AppointmentId}", appointmentId);
            await _billRepository.MarkAsPaidAsync(doctorId, appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Marks a bill as unpaid.
    /// </summary>
    public async Task<bool> MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as unpaid for appointment: {AppointmentId}", appointmentId);
            await _billRepository.MarkAsUnpaidAsync(doctorId, appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
