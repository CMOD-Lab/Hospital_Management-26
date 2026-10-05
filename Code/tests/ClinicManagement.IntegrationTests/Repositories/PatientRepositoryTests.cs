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
    public async Task AddAsync_ShouldAddPatient()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Test Patient",
            Email = "test@test.com",
            Password = "password",
            Phone = "1234567890",
            Gender = "M",
            Address = "Test Address",
            BirthDate = new DateTime(1990, 1, 1),
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(patient);

        // Assert
        result.PatientId.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Patient");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnPatient()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Test Patient",
            Email = "test2@test.com",
            Password = "password",
            IsActive = true
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(patient.PatientId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Patient");
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Test",
            Email = "exists@test.com",
            Password = "password",
            IsActive = true
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("exists@test.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActivePatients()
    {
        // Arrange
        _context.Patients.AddRange(
            new Patient { Name = "Active", Email = "active@test.com", Password = "p", IsActive = true },
            new Patient { Name = "Inactive", Email = "inactive@test.com", Password = "p", IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().OnlyContain(p => p.IsActive);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
