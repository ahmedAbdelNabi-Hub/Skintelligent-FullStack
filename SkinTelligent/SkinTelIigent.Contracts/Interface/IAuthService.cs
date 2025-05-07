using SkinTelIigent.Contracts.DTOs.Admin;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigentContracts.CustomResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.Interface
{
    public interface IAuthService
    {
        Task<BaseApiResponse> loginAsync(LoginDTO loginDTO);
        Task<BaseApiResponse> registerDoctorAsync(ApplicationUser doctor,string Password);
        Task<BaseApiResponse> registerPatientAsync(ApplicationUser patient, string Password);
        Task<BaseApiResponse> registerClinicAsync(ApplicationUser clinic, string password);
        Task<BaseApiResponse> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDto);
        Task<BaseApiResponse> RestPasswordAsync(RestPasswordDTO restPasswordDto);
        Task<BaseApiResponse> ForgotPasswordAsync(ForgetPasswordDTO confirmEmailDto);
        Task<BaseApiResponse> CreateAdminUser(AdminCreateDTO registerAdminDTO);


          Task<BaseApiResponse> DeleteAccount<T>(int id) where T : BaseEntity, IUserAssociated;
          Task<bool> EmailExists(string email);


    }
}
