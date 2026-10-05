using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for PatientRepository using in-memory database.
/// </summary>
public class PatientRepositoryTests : IDisposable
{
    private readonly ClinicDbContext _context;
    private readonly PatientRepository _repository;

    public PatientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ClinicDbContext(options);
        var logger = new Mock<ILogger<PatientRepository>>().Object;
        _repository = new PatientRepository(_context, logger);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnPatient()
    {
        _context.LoginAccounts.Add(new LoginAccount
        {
            LoginId = 1,
            Email = "test2@test.com",
            Password = "password",
            Type = 1
        });
        _context.Patients.Add(new Patient
        {
            PatientId = 1,
            Name = "Test Patient",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = "M"
        });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Patient");
        result.Email.Should().Be("test2@test.com");
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        _context.LoginAccounts.Add(new LoginAccount
        {
            Email = "exists@test.com",
            Password = "password",
            Type = 1
        });
        await _context.SaveChangesAsync();

        var result = await _repository.EmailExistsAsync("exists@test.com");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPatients()
    {
        _context.LoginAccounts.Add(new LoginAccount { LoginId = 1, Email = "a@test.com", Password = "p", Type = 1 });
        _context.Patients.Add(new Patient { PatientId = 1, Name = "Active", BirthDate = DateTime.Today, Gender = "M" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(1);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
