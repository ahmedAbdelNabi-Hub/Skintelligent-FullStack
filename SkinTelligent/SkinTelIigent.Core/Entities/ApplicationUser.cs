using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {

        public string UserType {  get; set; } 

        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Clinic Clinic { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();


    }
}
