# Architecture Documentation

## Clean Architecture Overview

```
ClinicManagement.Domain          (innermost - no dependencies)
    ↑
ClinicManagement.Application     (depends on Domain)
    ↑
ClinicManagement.Infrastructure  (depends on Domain + Application)
    ↑
ClinicManagement.Web             (depends on Infrastructure + Application)
```

## Layer Responsibilities

### Domain Layer
- **Entities**: Patient, Doctor, Department, Appointment, Bill, OtherStaff
- **Interfaces**: Repository and service contracts
- **Exceptions**: Domain-specific exceptions

### Application Layer
- **Services**: PatientService, DoctorService, AppointmentService, AdminService, BillService
- **DTOs**: Data transfer objects for each entity
- **Extensions**: DI registration

### Infrastructure Layer
- **DbContext**: ClinicDbContext with EF Core
- **Repositories**: EF Core implementations of domain interfaces
- **Configurations**: Entity type configurations
- **Extensions**: DI registration

### Web Layer
- **Razor Pages**: UI pages organized by role (Admin, Doctor, Patient)
- **Program.cs**: Application startup and middleware configuration
- **appsettings.json**: Configuration
- **wwwroot**: Static files (CSS, JS)
