using Microsoft.OpenApi.Models;
using UnitedAirlinesAPI.Services;

namespace UnitedAirlinesAPI.Infrastructure
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Use IHttpClientFactory directly
            services.AddHttpClient();

            // Add swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "United Airlines API",
                    Description = "Backend API for Alaska Airlines flight request",
                });
            });

            //add configuration
            services.Configure<SettingConfig>(configuration.GetSection("SettingConfig"));

            //register services
            services.AddScoped<ICargoService, CargoService>();
            return services;
        }
    }
}
