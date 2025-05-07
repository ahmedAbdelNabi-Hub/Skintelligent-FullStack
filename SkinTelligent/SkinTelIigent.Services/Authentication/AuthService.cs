using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Contracts.DTOs.Admin;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigentContracts.CustomResponses;
using SkinTelligent.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;    
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork,IEmailService emailService,RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> loginAsync(LoginDTO loginDTO)
        {
            var exsitEmail = await _userManager.LoadDoctorAndPatientUsersAsync(loginDTO.Email);

            if (exsitEmail == null)
                return AuthErrorResponse("The email is incorrect.");

            if (!exsitEmail.EmailConfirmed)
                return AuthErrorResponse("Email is not confirmed. Please confirm your email to log in.");

            if (exsitEmail.Doctor != null && exsitEmail.Doctor.IsApproved == false)
                return AuthErrorResponse("Email is not approved yet. Please wait!");

            if (!exsitEmail.LockoutEnabled)
                return AuthErrorResponse("Your account has been blocked. Please contact support.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(exsitEmail, loginDTO.Password);
            if (!isPasswordValid)
                return AuthErrorResponse("The password is incorrect.");

            return await _jwtService.CreateJwtToken(exsitEmail);
        }



        public async Task<BaseApiResponse> registerDoctorAsync(ApplicationUser doctor, string password)
        {
            return await RegisterUserAsync(doctor, password, "doctor");
        }

        public async Task<BaseApiResponse> registerPatientAsync(ApplicationUser patient, string password)
        {
            return await RegisterUserAsync(patient, password, "patient");
        }

        public async Task<BaseApiResponse> registerClinicAsync(ApplicationUser clinic, string password)
        {
            return await RegisterUserAsync(clinic, password,"clinic");
        }

        public async Task<BaseApiResponse> ForgotPasswordAsync(ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);
            if (user is null)
            {
                return new BaseApiResponse(StatusCodes.Status400BadRequest, "Email does not exist");
            }
             var otp = await GenerateTwoFactoryOTPAsync(user);
             return await SentOptCodeToEmail(user.Email!, "Password Reset Code Your OTP code " , $"{otp}");
        }

        public async Task<BaseApiResponse> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDto)
        {
           
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user is null)
            {
                return AuthErrorResponse("Email does not exist");
            }

            // Verify the provided OTP
            var isOtpValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, confirmEmailDto.OtpCode);
            if (!isOtpValid)
            {
                return AuthErrorResponse("Invalid or expired OTP. Please request a new one.");
            }
            user.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                return AuthSuccessResponse("The Email confirm successful");
            }

            return AuthErrorResponse( "An error occurred while confirming the email. Please try again.", StatusCodes.Status500InternalServerError);
        }

        public async Task<BaseApiResponse> RestPasswordAsync(RestPasswordDTO restPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(restPasswordDto.Email);
            if (user is null)
            {
                return new BaseApiResponse(StatusCodes.Status400BadRequest, "Email does not exist");
            }

            var isOtpValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, restPasswordDto.ResetOTP);
            if (!isOtpValid)
            {
                return new BaseApiResponse(StatusCodes.Status400BadRequest, "Invalid or expired OTP.");
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return new BaseApiResponse(StatusCodes.Status500InternalServerError, "Error removing the old password.");
            }

            result = await _userManager.AddPasswordAsync(user, restPasswordDto.Password);
            if (!result.Succeeded)
            {
                return new BaseApiResponse(StatusCodes.Status500InternalServerError, "Password reset failed.");
            }

            return new BaseApiResponse(StatusCodes.Status200OK, "Password reset successful.");
        }

        public async Task<BaseApiResponse> CreateAdminUser(AdminCreateDTO registerAdminDTO)
        {
            var admin = new ApplicationUser()
            {
                Email = registerAdminDTO.Email,
                UserName = registerAdminDTO.UserName,
                PhoneNumber = registerAdminDTO.Phone,
                EmailConfirmed = true,
                LockoutEnabled = false,
                UserType = "Admin"
            };

            var result = await _userManager.CreateAsync(admin, registerAdminDTO.Password);
            if (!result.Succeeded)
            {
                await _unitOfWork.RollbackAsync(); 
                return AuthErrorResponse("User registration failed. Please try again later.", StatusCodes.Status500InternalServerError);
            }

            var isAddRole = await _userManager.AddToRoleAsync(admin, "Admin");
            if (!isAddRole.Succeeded)
            {
                return AuthErrorResponse("Failed to assign admin role.", StatusCodes.Status500InternalServerError);
            }
            return new BaseApiResponse
            {
                statusCode = StatusCodes.Status200OK,
                message = "Admin user created successfully!"
            };
        }


        public async Task<BaseApiResponse> DeleteAccount<T>(int id) where T : BaseEntity , IUserAssociated
        {
            var entityRepo = _unitOfWork.Repository<T>();
            var entity = await entityRepo.GetByIdSpecAsync(new EntityWithUserSpecification<T>(id));
            if (entity == null)
                return new BaseApiResponse(404, $"{typeof(T).Name} not found.");
            await _unitOfWork.BeginTransactionAsync();

            var user = await _userManager.FindByIdAsync(entity.User.Id);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseApiResponse(404, "User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseApiResponse(400, "Failed to delete user.");
            }

            await _unitOfWork.CommitAsync();
            return new BaseApiResponse(200, $"{typeof(T).Name} and associated User deleted successfully.");


        }

        public async Task<bool> EmailExists(string email)
        {
           return await _userManager.FindByEmailAsync(email) is not null;
        }

        private async Task<BaseApiResponse> SentOptCodeToEmail(string email,string message ,string OtpCode)
        {
            var IsSentEmail = await _emailService.SendEmailAsync(email,message, OtpCode);
            if (IsSentEmail)
            {
                return AuthSuccessResponse("successful! A confirmation email has been sent to your inbox email.");
            }
            return AuthErrorResponse("An error occurred while sending the confirmation email.",StatusCodes.Status503ServiceUnavailable);
        }

        private async Task<string> GenerateTwoFactoryOTPAsync (ApplicationUser user)
        {
            var otpCode = await _userManager.GenerateTwoFactorTokenAsync(user,TokenOptions.DefaultEmailProvider); 
            return otpCode; 
        }

        private async Task<BaseApiResponse> RegisterUserAsync(ApplicationUser user, string password, string role)
        {
            if (user == null)
                return AuthErrorResponse("User registration failed. Please try again later.", StatusCodes.Status500InternalServerError);

       
            var existUser = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existUser != null)
                return AuthErrorResponse("The email is already associated with an account.");

            await _unitOfWork.BeginTransactionAsync();
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return AuthErrorResponse("User registration failed."+result.Errors.Select(d=>d.Code), StatusCodes.Status500InternalServerError);

            await _userManager.AddToRoleAsync(user, role);
            var otpTask = GenerateTwoFactoryOTPAsync(user);
            var emailTask = otpTask.ContinueWith(otp => SentOptCodeToEmail(user.Email!, "Confirm Your Email Address by OTP", otp.Result));

            await Task.WhenAll(otpTask, emailTask);
            await _unitOfWork.CommitAsync();

            return await (await emailTask);
        }

        private BaseApiResponse AuthErrorResponse(string message, int statusCode = 400)
        {
            return new BaseApiResponse
            {
                message = message,
                statusCode = statusCode
            };
        }
 
        private BaseApiResponse AuthSuccessResponse(string message, int statusCode = StatusCodes.Status200OK)
        {
            return new BaseApiResponse
            {
                message = message,
                statusCode = statusCode
            };
        }


    }
}
