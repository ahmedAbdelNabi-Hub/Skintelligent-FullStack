using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;

public class Doctor : BaseEntity , IUserAssociated 
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string LicenseNumber { get; set; }
    public int ExperienceYears { get; set; }
    public decimal DefaultExaminationFee { get; set; }
    public decimal DefaultConsultationFee { get; set; }
    public string ProfilePicture { get; set; }
    public string AboutMe { get; set; } = string.Empty;

    public bool IsProfileCompleted { get; set; } = false;
    public string Qualification { get; set; }
    public bool IsApproved { get; set; }

    public string Address { get; set; }

    public string Phone { get; set; }
    public string Email { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public ICollection<ClinicDoctor> ClinicDoctors { get; set; } = new HashSet<ClinicDoctor>();
    public ICollection<DoctorPatient> DoctorPatients { get; set; } = new HashSet<DoctorPatient>();

    public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
