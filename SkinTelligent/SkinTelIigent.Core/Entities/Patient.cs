using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;

public class Patient : BaseEntity , IUserAssociated
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public string ProfilePicture { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public string Phone { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public ICollection<DoctorPatient> DoctorPatients { get; set; } = new HashSet<DoctorPatient>();
    public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();

    public ICollection<Report> Reports { get; set; } = new HashSet<Report>();

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
