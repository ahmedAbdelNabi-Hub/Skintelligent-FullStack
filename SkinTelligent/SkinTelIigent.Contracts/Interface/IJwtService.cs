using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.Interface
{
    public interface IJwtService
    {
        Task<AuthResponse> CreateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();

    }
}
