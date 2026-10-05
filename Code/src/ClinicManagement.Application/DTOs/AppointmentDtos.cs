namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for appointment data transfer.
/// </summary>
public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int FreeSlot { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string? Timings { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public bool FeedbackGiven { get; set; }
}

public class AppointmentCreateDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlot { get; set; }
}

public class PrescriptionUpdateDto
{
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}

public class FreeSlotDto
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
}
