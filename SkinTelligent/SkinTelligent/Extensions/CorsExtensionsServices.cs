namespace SkinTelligent.Api.Extensions
{
    public static class CorsExtensionsServices
    {
        public static IServiceCollection AddCorsService(this IServiceCollection Services)
        {
            Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllWithCredentials", policy =>
                {
                    // Allow all origins with a specific header configuration and credentials
                    policy
                        .SetIsOriginAllowed(origin => true)  // Allow all origins
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();  // Allow credentials (cookies, auth tokens)
                });
            });
            return Services;
        }
    }
}
