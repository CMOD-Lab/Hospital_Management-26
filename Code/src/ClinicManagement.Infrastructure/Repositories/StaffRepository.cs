using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the staff repository.
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StaffId == id, cancellationToken);
    }

    public async Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        await using var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = new NpgsqlCommand(
            @"SELECT dbo.addstaff(
                @name, @birthdate, @phone, @gender, @designation, @address, @salary, @qualification)",
            connection);

        command.Parameters.AddWithValue("name", staff.Name);
        command.Parameters.Add(PostgresCommandHelper.DateParameter("birthdate", staff.BirthDate));
        command.Parameters.AddWithValue("phone", staff.Phone);
        command.Parameters.AddWithValue("gender", staff.Gender);
        command.Parameters.AddWithValue("designation", staff.Designation);
        command.Parameters.AddWithValue("address", staff.Address);
        command.Parameters.AddWithValue("salary", staff.Salary);
        command.Parameters.AddWithValue("qualification", staff.Qualification);

        await command.ExecuteNonQueryAsync(cancellationToken);

        var created = await _context.OtherStaff.AsNoTracking()
            .OrderByDescending(s => s.StaffId)
            .FirstAsync(cancellationToken);
        return created;
    }

    public async Task UpdateAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        _context.OtherStaff.Update(staff);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync("SELECT dbo.deletestaff({0})", id);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff.AnyAsync(s => s.StaffId == id, cancellationToken);
    }

    public async Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff.AsNoTracking()
            .Where(s => s.Name.Contains(searchQuery))
            .ToListAsync(cancellationToken);
    }
}
