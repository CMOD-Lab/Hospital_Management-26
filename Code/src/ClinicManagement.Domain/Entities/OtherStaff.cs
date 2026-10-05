using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents other staff members (non-doctor) in the clinic.
/// </summary>
public class OtherStaff
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;

    [NotMapped]
    public bool IsActive { get; set; } = true;

    [NotMapped]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
