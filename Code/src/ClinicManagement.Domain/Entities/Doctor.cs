using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a doctor in the clinic management system.
/// </summary>
public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;

    [NotMapped]
    public string Email { get; set; } = string.Empty;

    [NotMapped]
    public string Password { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int DeptNo { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public bool Status { get; set; } = true;

    [NotMapped]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Department? Department { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
