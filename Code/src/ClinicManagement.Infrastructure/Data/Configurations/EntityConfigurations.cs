using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClinicManagement.Infrastructure.Data.Configurations;

public class LoginAccountConfiguration : IEntityTypeConfiguration<LoginAccount>
{
    public void Configure(EntityTypeBuilder<LoginAccount> builder)
    {
        builder.ToTable("logintable", "dbo");
        builder.HasKey(l => l.LoginId);
        builder.Property(l => l.LoginId).HasColumnName("loginid").ValueGeneratedOnAdd();
        builder.Property(l => l.Password).HasColumnName("password").HasMaxLength(20).IsRequired();
        builder.Property(l => l.Email).HasColumnName("email").HasMaxLength(30).IsRequired();
        builder.Property(l => l.Type).HasColumnName("type");
    }
}

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient", "dbo");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("patientid").ValueGeneratedNever();
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(30).IsRequired();
        builder.Property(p => p.Phone).HasColumnName("phone").HasMaxLength(11);
        builder.Property(p => p.Address).HasColumnName("address").HasMaxLength(40);
        builder.Property(p => p.Gender).HasColumnName("gender").HasMaxLength(1);
        builder.Property(p => p.BirthDate).HasColumnName("birthdate").HasColumnType("date");
        builder.Ignore(p => p.Bills);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    private static readonly ValueConverter<bool, int> StatusConverter = new(
        v => v ? 1 : 0,
        v => v == 1);

    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctor", "dbo");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("doctorid").ValueGeneratedNever();
        builder.Property(d => d.Name).HasColumnName("name").HasMaxLength(30).IsRequired();
        builder.Property(d => d.Gender).HasColumnName("gender").HasMaxLength(1);
        builder.Property(d => d.Address).HasColumnName("address").HasMaxLength(40);
        builder.Property(d => d.Phone).HasColumnName("phone").HasMaxLength(11);
        builder.Property(d => d.BirthDate).HasColumnName("birthdate").HasColumnType("date");
        builder.Property(d => d.DeptNo).HasColumnName("deptno");
        builder.Property(d => d.ChargesPerVisit).HasColumnName("charges_per_visit")
            .HasConversion(v => (double)v, v => (int)v);
        builder.Property(d => d.Salary).HasColumnName("monthlysalary")
            .HasConversion(v => (double)v, v => (int)v);
        builder.Property(d => d.ReputeIndex).HasColumnName("reputeindex")
            .HasConversion(v => (double)v, v => (float)v);
        builder.Property(d => d.PatientsTreated).HasColumnName("patients_treated");
        builder.Property(d => d.Qualification).HasColumnName("qualification").HasMaxLength(100);
        builder.Property(d => d.Specialization).HasColumnName("specialization").HasMaxLength(100);
        builder.Property(d => d.Experience).HasColumnName("work_experience");
        builder.Property(d => d.Status).HasColumnName("status").HasConversion(StatusConverter);
        builder.HasOne(d => d.Department)
               .WithMany(dep => dep.Doctors)
               .HasForeignKey(d => d.DeptNo)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department", "dbo");
        builder.HasKey(d => d.DeptNo);
        builder.Property(d => d.DeptNo).HasColumnName("deptno").ValueGeneratedNever();
        builder.Property(d => d.DeptName).HasColumnName("deptname").HasMaxLength(30).IsRequired();
        builder.Property(d => d.Description).HasColumnName("description").HasMaxLength(1000);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    private static readonly ValueConverter<string, int> StatusConverter = new(
        v => LegacyAppointmentMapping.StatusFromString(v),
        v => LegacyAppointmentMapping.StatusToString(v));

    private static readonly ValueConverter<bool, int?> FeedbackConverter = new(
        v => v ? 1 : 2,
        v => v == 1);

    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointment", "dbo");
        builder.HasKey(a => a.AppointmentId);
        builder.Property(a => a.AppointmentId).HasColumnName("appointid").ValueGeneratedOnAdd();
        builder.Property(a => a.DoctorId).HasColumnName("doctorid");
        builder.Property(a => a.PatientId).HasColumnName("patientid");
        builder.Property(a => a.AppointmentDate).HasColumnName("date").HasColumnType("timestamp without time zone");
        builder.Property(a => a.Status).HasColumnName("appointment_status").HasConversion(StatusConverter);
        builder.Property(a => a.Disease).HasColumnName("disease").HasMaxLength(100);
        builder.Property(a => a.Progress).HasColumnName("progress").HasMaxLength(100);
        builder.Property(a => a.Prescription).HasColumnName("prescription").HasMaxLength(100);
        builder.Property(a => a.BillAmount).HasColumnName("bill_amount");
        builder.Property(a => a.BillStatus).HasColumnName("bill_status").HasMaxLength(10);
        builder.Property(a => a.PatientNotification).HasColumnName("patientnotification");
        builder.Property(a => a.FeedbackGiven).HasColumnName("feedbackstatus").HasConversion(FeedbackConverter);
        builder.HasOne(a => a.Doctor)
               .WithMany(d => d.Appointments)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(a => a.Bill);
    }
}

public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("otherstaff", "dbo");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("staffid").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(30).IsRequired();
        builder.Property(s => s.Phone).HasColumnName("phone").HasMaxLength(11);
        builder.Property(s => s.Gender).HasColumnName("gender").HasMaxLength(1);
        builder.Property(s => s.Address).HasColumnName("address").HasMaxLength(30);
        builder.Property(s => s.Designation).HasColumnName("designation").HasMaxLength(15);
        builder.Property(s => s.Qualification).HasColumnName("highest_qualification").HasMaxLength(50);
        builder.Property(s => s.BirthDate).HasColumnName("birthdate").HasColumnType("date");
        builder.Property(s => s.Salary).HasColumnName("salary")
            .HasConversion(v => (double)v, v => (int)v);
    }
}
