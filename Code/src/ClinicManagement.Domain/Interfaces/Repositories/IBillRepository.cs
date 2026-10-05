using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Bill entity operations.
/// </summary>
public interface IBillRepository
{
    Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Bill?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default);
    Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
}
