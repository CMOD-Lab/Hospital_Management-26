using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

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
        var patients = await _context.Patients.AsNoTracking().ToListAsync(cancellationToken);
        await LoginHelper.AttachLoginsAsync(_context, patients, cancellationToken);
        return patients;
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == id, cancellationToken);
        if (patient != null)
        {
            await LoginHelper.AttachLoginAsync(_context, patient, cancellationToken);
        }
        return patient;
    }

    public async Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var login = await _context.LoginAccounts.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Email == email && l.Type == 1, cancellationToken);
        if (login == null)
        {
            return null;
        }

        var patient = await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == login.LoginId, cancellationToken);
        if (patient == null)
        {
            return null;
        }

        patient.Email = login.Email;
        patient.Password = login.Password;
        return patient;
    }

    public async Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        var (status, id) = await SignupAsync(patient, cancellationToken);
        if (status != 1)
        {
            throw new InvalidOperationException("Unable to register patient.");
        }

        patient.PatientId = id;
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
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.PatientId == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginAccounts.AnyAsync(l => l.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        var patients = await _context.Patients.AsNoTracking()
            .Where(p => p.Name.Contains(searchQuery))
            .ToListAsync(cancellationToken);
        await LoginHelper.AttachLoginsAsync(_context, patients, cancellationToken);
        return patients;
    }

    public async Task<(int status, int id)> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var patient = await GetByEmailAsync(email, cancellationToken);
        if (patient == null) return (1, 0);
        if (patient.Password != password) return (2, 0);
        return (0, patient.PatientId);
    }

    public async Task<(int status, int id)> SignupAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        if (await EmailExistsAsync(patient.Email, cancellationToken))
        {
            return (0, 0);
        }

        await using var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = new NpgsqlCommand(
            "SELECT p_status, p_id FROM dbo.patientsignup(@name, @phone, @address, @date, @gender, @password, @email)",
            connection);

        command.Parameters.AddWithValue("name", patient.Name);
        command.Parameters.AddWithValue("phone", patient.Phone);
        command.Parameters.AddWithValue("address", patient.Address);
        command.Parameters.Add(PostgresCommandHelper.DateParameter("date", patient.BirthDate));
        command.Parameters.AddWithValue("gender", patient.Gender);
        command.Parameters.AddWithValue("password", patient.Password);
        command.Parameters.AddWithValue("email", patient.Email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return (-1, 0);
        }

        var status = reader.GetInt32(0);
        var id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
        return (status, id);
    }

    public async Task<(int status, int id)> ValidateAdminLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var login = await _context.LoginAccounts.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Email == email && l.Type == 3, cancellationToken);
        if (login == null)
        {
            return (1, 0);
        }

        if (login.Password != password)
        {
            return (2, 0);
        }

        return (0, login.LoginId);
    }
}
