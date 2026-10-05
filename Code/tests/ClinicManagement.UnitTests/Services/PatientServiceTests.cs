using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for PatientService.
/// </summary>
public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 1,
            Email = "test@test.com",
            Password = "password123",
            Name = "Test Patient",
            IsActive = true
        };
        _patientRepositoryMock.Setup(r => r.GetByEmailAsync("test@test.com", default))
            .ReturnsAsync(patient);

        var loginDto = new LoginDto { Email = "test@test.com", Password = "password123" };

        // Act
        var result = await _patientService.ValidateLoginAsync(loginDto);

        // Assert
        result.Status.Should().Be(0);
        result.UserId.Should().Be(1);
        result.UserType.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidEmail_ReturnsEmailNotFound()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.GetByEmailAsync("notfound@test.com", default))
            .ReturnsAsync((Patient?)null);

        var loginDto = new LoginDto { Email = "notfound@test.com", Password = "password123" };

        // Act
        var result = await _patientService.ValidateLoginAsync(loginDto);

        // Assert
        result.Status.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithWrongPassword_ReturnsWrongPassword()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 1,
            Email = "test@test.com",
            Password = "correctpassword",
            Name = "Test Patient"
        };
        _patientRepositoryMock.Setup(r => r.GetByEmailAsync("test@test.com", default))
            .ReturnsAsync(patient);

        var loginDto = new LoginDto { Email = "test@test.com", Password = "wrongpassword" };

        // Act
        var result = await _patientService.ValidateLoginAsync(loginDto);

        // Assert
        result.Status.Should().Be(2);
    }

    [Fact]
    public async Task SignupAsync_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.EmailExistsAsync("new@test.com", default))
            .ReturnsAsync(false);
        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), default))
            .ReturnsAsync(new Patient { PatientId = 5, Email = "new@test.com" });

        var dto = new PatientCreateDto
        {
            Name = "New Patient",
            Email = "new@test.com",
            Password = "password",
            BirthDate = "1990-01-01",
            Gender = "M",
            Phone = "1234567890",
            Address = "Test Address"
        };

        // Act
        var (status, id) = await _patientService.SignupAsync(dto);

        // Assert
        status.Should().Be(1);
        id.Should().Be(5);
    }

    [Fact]
    public async Task SignupAsync_WithExistingEmail_ReturnsEmailExists()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.EmailExistsAsync("existing@test.com", default))
            .ReturnsAsync(true);

        var dto = new PatientCreateDto
        {
            Name = "Test",
            Email = "existing@test.com",
            Password = "password"
        };

        // Act
        var (status, id) = await _patientService.SignupAsync(dto);

        // Assert
        status.Should().Be(0);
        id.Should().Be(0);
    }

    [Fact]
    public async Task GetPatientByIdAsync_WithValidId_ReturnsPatient()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 1,
            Name = "Test Patient",
            Email = "test@test.com",
            Phone = "1234567890",
            Gender = "M",
            Address = "Test Address",
            BirthDate = new DateTime(1990, 1, 1),
            IsActive = true
        };
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPatientByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.PatientId.Should().Be(1);
        result.Name.Should().Be("Test Patient");
    }
}
