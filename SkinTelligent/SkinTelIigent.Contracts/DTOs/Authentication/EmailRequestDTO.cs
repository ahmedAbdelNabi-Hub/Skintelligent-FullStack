using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Authentication
{
    public class EmailRequestDTO
    {
        [Required, EmailAddress]
        public string EmailTo { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public IList<IFormFile> Attachments { get; set; } = new List<IFormFile>();
    }
}
