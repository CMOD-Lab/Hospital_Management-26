using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patient");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("PatientID").ValueGeneratedOnAdd();
        builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(15);
        builder.Property(p => p.Address).HasMaxLength(40);
        builder.Property(p => p.Gender).HasMaxLength(1);
        builder.Property(p => p.Email).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Password).HasMaxLength(20).IsRequired();
        builder.Property(p => p.IsActive).HasDefaultValue(true);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctor");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("DoctorID").ValueGeneratedOnAdd();
        builder.Property(d => d.Name).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Email).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Password).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Gender).HasMaxLength(1);
        builder.Property(d => d.Address).HasMaxLength(30);
        builder.Property(d => d.Phone).HasMaxLength(30);
        builder.Property(d => d.Specialization).HasMaxLength(30);
        builder.Property(d => d.Qualification).HasMaxLength(30);
        builder.Property(d => d.Status).HasDefaultValue(true);
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
        builder.ToTable("Department");
        builder.HasKey(d => d.DeptNo);
        builder.Property(d => d.DeptNo).ValueGeneratedOnAdd();
        builder.Property(d => d.DeptName).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(200);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointment");
        builder.HasKey(a => a.AppointmentId);
        builder.Property(a => a.AppointmentId).HasColumnName("AppointmentID").ValueGeneratedOnAdd();
        builder.Property(a => a.Status).HasMaxLength(20).HasDefaultValue("Pending");
        builder.Property(a => a.Timings).HasMaxLength(30);
        builder.Property(a => a.Disease).HasMaxLength(30);
        builder.Property(a => a.Progress).HasMaxLength(50);
        builder.Property(a => a.Prescription).HasMaxLength(60);
        builder.HasOne(a => a.Doctor)
               .WithMany(d => d.Appointments)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("Bill");
        builder.HasKey(b => b.BillId);
        builder.Property(b => b.BillId).HasColumnName("BillID").ValueGeneratedOnAdd();
        builder.Property(b => b.Amount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.Description).HasMaxLength(200);
        builder.HasOne(b => b.Patient)
               .WithMany(p => p.Bills)
               .HasForeignKey(b => b.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Appointment)
               .WithOne(a => a.Bill)
               .HasForeignKey<Bill>(b => b.AppointmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("OtherStaff");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("StaffID").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(30);
        builder.Property(s => s.Gender).HasMaxLength(1);
        builder.Property(s => s.Address).HasMaxLength(50);
        builder.Property(s => s.Designation).HasMaxLength(30);
        builder.Property(s => s.Qualification).HasMaxLength(30);
        builder.Property(s => s.IsActive).HasDefaultValue(true);
    }
}
