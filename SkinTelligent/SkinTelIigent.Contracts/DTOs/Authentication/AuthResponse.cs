using SkinTelIigent.Contracts.DTOs;
using SkinTelIigentContracts.CustomResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Authentication
{
    public class AuthResponse : BaseApiResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; } = DateTime.MinValue; 
    }
}
