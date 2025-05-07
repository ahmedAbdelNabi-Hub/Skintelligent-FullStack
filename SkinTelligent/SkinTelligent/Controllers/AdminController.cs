using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Contracts.DTOs.Admin;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigentContracts.CustomResponses;

namespace SkinTelligent.Api.Controllers
{
    public class AdminController:BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService ;
        public AdminController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _authService = authService;
            _roleManager = roleManager;
        }

        [HttpGet("/api/admins")]
        public async Task<ActionResult<AdminDTO>> GetAllAdmins()
        {
            var admins = await _userManager.Users
                .Where(u => u.UserType=="Admin")
                .Select(u => new AdminDTO() { Id = u.Id ,Email = u.Email!,UserName = u.UserName!, LockoutEnabled = u.LockoutEnabled , Phone = u.PhoneNumber! })
                .ToListAsync();

            return Ok(admins);
        }


        [HttpPost("/api/admin")]
        public async Task<ActionResult<BaseApiResponse>> CreateAdmin([FromBody] AdminCreateDTO model)
        {
            var result = await _authService.CreateAdminUser(model);
            return HandleStatusCode(result);
        }
        
        [HttpPut("/api/admins/{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] AdminDTO model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "Admin not found"));

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new BaseApiResponse(StatusCodes.Status400BadRequest, "Failed to update admin"));

            return Ok(new BaseApiResponse(StatusCodes.Status200OK, "Admin updated successfully"));
        }

        [HttpDelete("/api/admins/{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "Admin not found"));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new BaseApiResponse(StatusCodes.Status400BadRequest, "Failed to delete admin"));

            return Ok(new BaseApiResponse(StatusCodes.Status200OK, "Admin deleted successfully"));
        }


        [HttpPost("/api/admins/{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDTO model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "Admin not found"));

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new BaseApiResponse(StatusCodes.Status400BadRequest, "Failed to change password"));

            return Ok(new BaseApiResponse(StatusCodes.Status200OK, "Password changed successfully"));
        }
    }
}
