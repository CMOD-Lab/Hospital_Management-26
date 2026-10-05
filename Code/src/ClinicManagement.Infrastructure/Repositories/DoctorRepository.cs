using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the doctor repository.
/// </summary>
public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DoctorRepository> _logger;

    public DoctorRepository(ClinicDbContext context, ILogger<DoctorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await _context.Doctors.AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Status)
            .ToListAsync(cancellationToken);
        await LoginHelper.AttachLoginsAsync(_context, doctors, cancellationToken);
        return doctors;
    }

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors.AsNoTracking()
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.DoctorId == id, cancellationToken);
        if (doctor != null)
        {
            await LoginHelper.AttachLoginAsync(_context, doctor, cancellationToken);
        }
        return doctor;
    }

    public async Task<Doctor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var login = await _context.LoginAccounts.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Email == email && l.Type == 2, cancellationToken);
        if (login == null)
        {
            return null;
        }

        var doctor = await _context.Doctors.AsNoTracking()
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.DoctorId == login.LoginId, cancellationToken);
        if (doctor == null)
        {
            return null;
        }

        doctor.Email = login.Email;
        doctor.Password = login.Password;
        return doctor;
    }

    public async Task<Doctor> AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        await using var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = new NpgsqlCommand(
            @"SELECT dbo.adddoctor(
                @name, @email, @password, @birthdate, @dept, @gender, @address,
                @exp, @salary, @spec, @phone, @charges, @qual)",
            connection);

        command.Parameters.AddWithValue("name", doctor.Name);
        command.Parameters.AddWithValue("email", doctor.Email);
        command.Parameters.AddWithValue("password", doctor.Password);
        command.Parameters.Add(PostgresCommandHelper.DateParameter("birthdate", doctor.BirthDate));
        command.Parameters.AddWithValue("dept", doctor.DeptNo);
        command.Parameters.AddWithValue("gender", doctor.Gender.Length > 0 ? doctor.Gender[0].ToString() : "M");
        command.Parameters.AddWithValue("address", doctor.Address);
        command.Parameters.AddWithValue("exp", doctor.Experience);
        command.Parameters.AddWithValue("salary", doctor.Salary);
        command.Parameters.AddWithValue("spec", doctor.Specialization);
        command.Parameters.AddWithValue("phone", doctor.Phone);
        command.Parameters.AddWithValue("charges", doctor.ChargesPerVisit);
        command.Parameters.AddWithValue("qual", doctor.Qualification);

        await command.ExecuteNonQueryAsync(cancellationToken);

        var login = await _context.LoginAccounts.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Email == doctor.Email && l.Type == 2, cancellationToken);
        if (login != null)
        {
            doctor.DoctorId = login.LoginId;
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE dbo.doctor SET status = 1 WHERE doctorid = {0}", login.LoginId);
            doctor.Status = true;
        }

        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT dbo.deletedoctor({0})", id);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.AnyAsync(d => d.DoctorId == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginAccounts.AnyAsync(l => l.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        var doctors = await _context.Doctors.AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Status && d.Name.Contains(searchQuery))
            .ToListAsync(cancellationToken);
        await LoginHelper.AttachLoginsAsync(_context, doctors, cancellationToken);
        return doctors;
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        var doctors = await _context.Doctors.AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Status && d.Department != null && d.Department.DeptName == deptName)
            .ToListAsync(cancellationToken);
        await LoginHelper.AttachLoginsAsync(_context, doctors, cancellationToken);
        return doctors;
    }
}
