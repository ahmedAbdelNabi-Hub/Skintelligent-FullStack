using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;

public class Clinic : BaseEntity , IUserAssociated
{
    public string ClinicName { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Image { get; set; } = "default.jpg";
    public string ContactNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsApproved { get; set; } = false;   

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public ICollection<ClinicDoctor> ClinicDoctors { get; set; } = new HashSet<ClinicDoctor>();
    public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();


}
