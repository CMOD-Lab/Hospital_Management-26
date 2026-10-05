using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Data;

internal static class LoginHelper
{
    public static async Task AttachLoginAsync(ClinicDbContext context, Patient patient, CancellationToken cancellationToken)
    {
        var login = await context.LoginAccounts.AsNoTracking()
            .FirstOrDefaultAsync(l => l.LoginId == patient.PatientId && l.Type == 1, cancellationToken);
        if (login != null)
        {
            patient.Email = login.Email;
            patient.Password = login.Password;
        }
    }

    public static async Task AttachLoginAsync(ClinicDbContext context, Doctor doctor, CancellationToken cancellationToken)
    {
        var login = await context.LoginAccounts.AsNoTracking()
            .FirstOrDefaultAsync(l => l.LoginId == doctor.DoctorId && l.Type == 2, cancellationToken);
        if (login != null)
        {
            doctor.Email = login.Email;
            doctor.Password = login.Password;
        }
    }

    public static async Task AttachLoginsAsync(ClinicDbContext context, IEnumerable<Patient> patients, CancellationToken cancellationToken)
    {
        var ids = patients.Select(p => p.PatientId).ToList();
        var logins = await context.LoginAccounts.AsNoTracking()
            .Where(l => ids.Contains(l.LoginId) && l.Type == 1)
            .ToDictionaryAsync(l => l.LoginId, cancellationToken);

        foreach (var patient in patients)
        {
            if (logins.TryGetValue(patient.PatientId, out var login))
            {
                patient.Email = login.Email;
                patient.Password = login.Password;
            }
        }
    }

    public static async Task AttachLoginsAsync(ClinicDbContext context, IEnumerable<Doctor> doctors, CancellationToken cancellationToken)
    {
        var ids = doctors.Select(d => d.DoctorId).ToList();
        var logins = await context.LoginAccounts.AsNoTracking()
            .Where(l => ids.Contains(l.LoginId) && l.Type == 2)
            .ToDictionaryAsync(l => l.LoginId, cancellationToken);

        foreach (var doctor in doctors)
        {
            if (logins.TryGetValue(doctor.DoctorId, out var login))
            {
                doctor.Email = login.Email;
                doctor.Password = login.Password;
            }
        }
    }

    public static void ApplyFreeSlot(Appointment appointment)
    {
        if (appointment.AppointmentDate != default)
        {
            appointment.FreeSlot = LegacyAppointmentMapping.GetFreeSlotFromDate(appointment.AppointmentDate);
            appointment.Timings = appointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
