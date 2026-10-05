using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the appointment repository.
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    private static void Enrich(Appointment appointment)
    {
        LoginHelper.ApplyFreeSlot(appointment);
    }

    private static void EnrichAll(IEnumerable<Appointment> appointments)
    {
        foreach (var appointment in appointments)
        {
            Enrich(appointment);
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _context.Appointments.AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .ToListAsync(cancellationToken);
        EnrichAll(list);
        return list;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.AppointmentId == id, cancellationToken);
        if (appointment != null)
        {
            Enrich(appointment);
        }
        return appointment;
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        appointment.AppointmentDate = LegacyAppointmentMapping.BuildAppointmentTimestamp(appointment.FreeSlot);
        appointment.Status = "Pending";
        appointment.PatientNotification = 1;
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        Enrich(appointment);
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        if (appointment != null)
        {
            appointment.Status = "Cancelled";
            appointment.PatientNotification = 2;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.AnyAsync(a => a.AppointmentId == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Appointments.AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        EnrichAll(list);
        return list;
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Appointments.AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        EnrichAll(list);
        return list;
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Appointments.AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.Status == "Pending")
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        EnrichAll(list);
        return list;
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var list = await _context.Appointments.AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId
                && a.AppointmentDate.Date == today
                && a.Status == "Approved")
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        EnrichAll(list);
        return list;
    }

    public async Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var appointment = await _context.Appointments.AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId
                && a.AppointmentDate.Date == today
                && (a.Status == "Approved" || a.Status == "Pending"))
            .OrderByDescending(a => a.AppointmentDate)
            .FirstOrDefaultAsync(cancellationToken);
        if (appointment != null)
        {
            Enrich(appointment);
        }
        return appointment;
    }

    public async Task<Appointment?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.AsNoTracking()
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.PatientId == patientId
                && a.Status == "Completed"
                && !a.FeedbackGiven, cancellationToken);
        if (appointment != null)
        {
            Enrich(appointment);
        }
        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetNotificationsByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Appointments.AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId && a.PatientNotification == 2)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
        EnrichAll(list);
        return list;
    }

    public async Task<IEnumerable<int>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var bookedHours = await _context.Appointments.AsNoTracking()
            .Where(a => a.DoctorId == doctorId
                && a.AppointmentDate.Date == today
                && a.Status != "Cancelled")
            .Select(a => a.AppointmentDate.Hour)
            .ToListAsync(cancellationToken);

        return Enumerable.Range(1, 10)
            .Where(slot => !bookedHours.Contains(LegacyAppointmentMapping.SlotToHour(slot)));
    }

    public async Task ApproveAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { appointmentId }, cancellationToken);
        if (appointment != null)
        {
            appointment.Status = "Approved";
            appointment.PatientNotification = 2;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { appointmentId }, cancellationToken);
        if (appointment != null)
        {
            appointment.FeedbackGiven = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.DoctorId == doctorId, cancellationToken);
        if (appointment != null)
        {
            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            appointment.Status = "Completed";
            appointment.PatientNotification = 2;
            appointment.FeedbackGiven = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
