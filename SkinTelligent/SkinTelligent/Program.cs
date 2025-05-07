
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Infrastructure.Data.SeedData;
using SkinTelIigent.Infrastructure.Repositories;
using SkinTelligent.Api.Extensions;
using SkinTelligent.Api.Middlewares;
using SkinTelligent.Api.Hubs;
using System.Text.Json.Serialization;
using Hangfire;
using SkinTelIigent.Contracts.Interface;


namespace SkinTelligent
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddHangfireServices(builder.Configuration);
            builder.Services.AddControllers(options => options.Filters.Add<ValidateModelAttribute>())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //IgnoreCycles
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token with **Bearer** prefix. Example: `Bearer eyJhbGciOiJI...`"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.Configure<ApiBehaviorOptions>(option => option.SuppressModelStateInvalidFilter = true);
            builder.Services.AddCorsService();
            builder.Services.AddSignalR();


            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Information);


            // Extensions Services 
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.IdentityExtentionService();
            builder.Services.JwtService(builder.Configuration);
            builder.Services.AddAuthorization();

            var app = builder.Build();

            #region Update Database && Add SeedData && recurringJobManager

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await RoleSeeder.SeedRoles(services);
                var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();
                recurringJobManager.AddOrUpdate<IAppointmentService>(
                    "DailyAppointmentCompletionJob",
                    x => x.ProcessAppointmentsAsync(),
                     Cron.Minutely(),
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }
                );
            }
            #endregion

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkinTelligent API V1");
                    options.RoutePrefix = "docs";

                });
            }
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".avif"] = "image/avif"; 
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider 
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowAllWithCredentials");  

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/hubs/notifications");
            app.UseMiddleware<GlobalExceptionHandling>();
            app.MapControllers();

            app.Run();
        }
    }
}
