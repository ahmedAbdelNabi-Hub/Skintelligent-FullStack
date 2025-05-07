using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Entities;

namespace SkinTelligent.Api.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<ApplicationUser?> LoadDoctorAndPatientUsersAsync(
            this UserManager<ApplicationUser> userManager, string email)
        {
            var user = await userManager.Users
                .Include(u => u.Doctor) 
                .Include(u => u.Patient) 
                .FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }
    }
}
