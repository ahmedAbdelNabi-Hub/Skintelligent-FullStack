using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Admin
{
    public class AdminDTO 
    {
        public string? Id { get; set; }
        public string UserName { get; set; }  
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? LockoutEnabled { get; set; }

    }
}
