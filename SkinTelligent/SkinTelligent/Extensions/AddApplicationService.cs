using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Infrastructure.Data;
using SkinTelIigent.Infrastructure.Repositories;
using SkinTelIigent.Infrastructure.UnitOfWork;
using SkinTelIigent.Services;
using SkinTelIigent.Services.Authentication;
using SkinTelIigent.Services.SentEmail;
using SkinTelligent.Api.Helper.MappingProfile;
using SkinTelligent.Api.Hubs;
using SkinTelligent.Api.RealTime;

namespace SkinTelligent.Api.Extensions
{
    public static class AddApplicationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) {

            services.AddDbContext<SkinTelIigentDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });    
            
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IAuthService, AuthService>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IAppointmentService), typeof(AppointmentService));
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddScoped<INotificationSender, SignalRNotificationSender>();



            return services;    
        }
    }
}
