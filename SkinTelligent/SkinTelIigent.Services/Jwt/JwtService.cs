using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.DTOs.Jwt;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Core.Specification.PatientSpecific;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtConfig _jwt;
        private readonly IUnitOfWork _unitOfWork;

        public JwtService(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JwtConfig> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponse> CreateJwtToken(ApplicationUser user)
        {
            var claims = await setClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.Expiration),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponse
            {
                statusCode = 200,
                Token = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<List<Claim>> setClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in userRoles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                roleClaims.Add(roleClaim);
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Name,user.UserName!)

            };
            var doctor = await _unitOfWork.Repository<Doctor>()
                        .GetByIdSpecAsync(new DoctorSpecifications(user.Id));

            var patient = doctor == null
                ? await _unitOfWork.Repository<Patient>().GetByIdSpecAsync(new PatientSpecification(user.Id))
                : null;

            var clinic = (doctor == null && patient == null)
                ? await _unitOfWork.Repository<Clinic>().GetByIdSpecAsync(new ClinicSpecifications(user.Id))
                : null;

            if (doctor != null)
                claims.Add(new Claim("doctorId", doctor.Id.ToString()));

            if (patient != null)
                claims.Add(new Claim("patientId", patient.Id.ToString()));

            if (clinic != null)
                claims.Add(new Claim("clinicId", clinic.Id.ToString()));

            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            return claims;
        }
    }
}