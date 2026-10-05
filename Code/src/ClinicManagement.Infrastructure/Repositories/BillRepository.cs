using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the bill repository.
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

    public async Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bills.AsNoTracking()
            .Include(b => b.Patient)
            .Include(b => b.Appointment).ThenInclude(a => a!.Doctor)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bill?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bills.AsNoTracking()
            .Include(b => b.Patient)
            .Include(b => b.Appointment)
            .FirstOrDefaultAsync(b => b.BillId == id, cancellationToken);
    }

    public async Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync(cancellationToken);
        return bill;
    }

    public async Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        _context.Bills.Update(bill);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var bill = await _context.Bills.FindAsync(new object[] { id }, cancellationToken);
        if (bill != null)
        {
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills.AsNoTracking()
            .Include(b => b.Appointment).ThenInclude(a => a!.Doctor)
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills.AsNoTracking()
            .Include(b => b.Patient)
            .Include(b => b.Appointment)
            .Where(b => b.Appointment != null && b.Appointment.DoctorId == doctorId && !b.IsPaid)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        var bill = await _context.Bills
            .Include(b => b.Appointment)
            .FirstOrDefaultAsync(b => b.AppointmentId == appointmentId && b.Appointment != null && b.Appointment.DoctorId == doctorId, cancellationToken);
        if (bill != null)
        {
            bill.IsPaid = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        var bill = await _context.Bills
            .Include(b => b.Appointment)
            .FirstOrDefaultAsync(b => b.AppointmentId == appointmentId && b.Appointment != null && b.Appointment.DoctorId == doctorId, cancellationToken);
        if (bill != null)
        {
            bill.IsPaid = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
