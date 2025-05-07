using Microsoft.AspNetCore.Identity;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Infrastructure.Data;

namespace SkinTelligent.Api.Extensions
{
    public static class IdentityExtentionServices
    {
        public static IServiceCollection IdentityExtentionService(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;

                options.Password.RequireDigit = false;  
                options.Password.RequiredLength = 6;    
                options.Password.RequireLowercase = false; 
                options.Password.RequireUppercase = false; 
                options.Password.RequireNonAlphanumeric = false; 

            })
            .AddEntityFrameworkStores<SkinTelIigentDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options => {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });

            return services;
        }
    }
}
