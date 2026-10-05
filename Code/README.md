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
- SQL Server or SQL Server Express
- Visual Studio 2022 or VS Code

### Database Setup
1. Update the connection string in `src/ClinicManagement.Web/appsettings.json`
2. Run the existing SQL scripts from `Database Files/` folder
3. Or run EF Core migrations: `dotnet ef database update --project src/ClinicManagement.Infrastructure`

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
