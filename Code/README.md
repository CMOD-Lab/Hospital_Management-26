# Clinic Management System - .NET 8 Migration

## Overview
This is a Clinic Management System migrated from ASP.NET Web Forms 4.5.2 to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:
- **Domain**: Entities, interfaces, domain exceptions
- **Application**: Business logic services, DTOs
- **Infrastructure**: EF Core repositories, database context
- **Web**: Razor Pages UI layer

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- PostgreSQL 14+ (local or AWS RDS)
- Visual Studio 2022 or VS Code

### Database Setup
1. Update the connection string in `src/ClinicManagement.Web/appsettings.json`
2. Apply `postgresql-schema.sql` from the repository root to your PostgreSQL database
3. See `docs/TRANSFORMATION_GAP_AND_REMEDIATION.md` for why code changes were required and how to run on PostgreSQL/RDS

### Running the Application
```bash
cd src/ClinicManagement.Web
dotnet run
```

### Running Tests
```bash
dotnet test
```

## Migration Notes
- Web Forms pages migrated to Razor Pages
- ADO.NET replaced with Entity Framework Core 8.0
- Web.config replaced with appsettings.json
- Global.asax replaced with Program.cs
- Session state preserved using ASP.NET Core session middleware
- Bootstrap 5 used for UI (upgraded from Bootstrap 3)

## Default Admin Credentials
- Email: admin@clinic.com
- Password: admin123
