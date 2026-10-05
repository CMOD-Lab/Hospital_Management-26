namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents an appointment between a patient and a doctor.
/// </summary>
public class Appointment
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlot { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Completed, Cancelled
    public DateTime AppointmentDate { get; set; }
    public string? Timings { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public bool FeedbackGiven { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Bill? Bill { get; set; }
}
