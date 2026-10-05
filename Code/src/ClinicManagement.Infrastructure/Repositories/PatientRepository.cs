using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the patient repository.
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<PatientRepository> _logger;

    public PatientRepository(ClinicDbContext context, ILogger<PatientRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AsNoTracking()
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == id, cancellationToken);
    }

    public async Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);
        return patient;
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.FindAsync(new object[] { id }, cancellationToken);
        if (patient != null)
        {
            patient.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.PatientId == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AsNoTracking()
            .Where(p => p.IsActive && p.Name.Contains(searchQuery))
            .ToListAsync(cancellationToken);
    }

    public async Task<(int status, int id)> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
        if (patient == null) return (1, 0);
        if (patient.Password != password) return (2, 0);
        return (0, patient.PatientId);
    }

    public async Task<(int status, int id)> SignupAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        var exists = await EmailExistsAsync(patient.Email, cancellationToken);
        if (exists) return (0, 0);
        var created = await AddAsync(patient, cancellationToken);
        return (1, created.PatientId);
    }
}
