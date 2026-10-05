using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// Bill data is stored on dbo.appointment (bill_amount, bill_status).
/// </summary>
public class BillRepository : IBillRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<BillRepository> _logger;

    public BillRepository(ClinicDbContext context, ILogger<BillRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    private static Bill MapFromAppointment(Appointment a)
    {
        LoginHelper.ApplyFreeSlot(a);
        return new Bill
        {
            BillId = a.AppointmentId,
            PatientId = a.PatientId,
            AppointmentId = a.AppointmentId,
            Amount = (decimal)(a.BillAmount ?? 0),
            IsPaid = string.Equals(a.BillStatus, "paid", StringComparison.OrdinalIgnoreCase),
            BillDate = a.AppointmentDate,
            Description = a.Disease,
            Patient = a.Patient,
            Appointment = a
        };
    }

    private IQueryable<Appointment> BillAppointments =>
        _context.Appointments.AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.BillAmount != null);

    public async Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var appointments = await BillAppointments.ToListAsync(cancellationToken);
        return appointments.Select(MapFromAppointment);
    }

    public async Task<Bill?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await BillAppointments.FirstOrDefaultAsync(a => a.AppointmentId == id, cancellationToken);
        return appointment == null ? null : MapFromAppointment(appointment);
    }

    public Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Bills are created via appointment billing fields.");
    }

    public Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use MarkAsPaidAsync or MarkAsUnpaidAsync.");
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Bills are not deleted separately.");
    }

    public async Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var appointments = await BillAppointments
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        return appointments.Select(MapFromAppointment);
    }

    public async Task<IEnumerable<Bill>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var appointments = await _context.Appointments.AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId
                && a.Status == "Completed"
                && (a.BillAmount == null || a.BillStatus == null))
            .ToListAsync(cancellationToken);
        return appointments.Select(a => new Bill
        {
            BillId = a.AppointmentId,
            PatientId = a.PatientId,
            AppointmentId = a.AppointmentId,
            Amount = 0,
            IsPaid = false,
            BillDate = a.AppointmentDate,
            Patient = a.Patient,
            Appointment = a
        });
    }

    public async Task MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT dbo.finishedpaid({0}, {1})", doctorId, appointmentId);
    }

    public async Task MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT dbo.finishedunpaid({0}, {1})", doctorId, appointmentId);
    }
}
