using Microsoft.OpenApi.Models;
using UnitedAirlinesAPI.Services;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.CookiePolicy;
using Constants = UnitedAirlinesAPI.Utilities.Constants;

namespace UnitedAirlinesAPI.Infrastructure;

public static class DependencyExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        //Use IHttpClientFactory directly
        services.AddHttpClient();

        //Use named httpClient
        services.ConfigureHttpClientWithCertificate(Constants.APIClient, configuration);

        //set cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            //This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Lax; //the client should send the cookie with "same-site" requests, and with "cross-site" top-level navigations
            options.HttpOnly = HttpOnlyPolicy.Always; //cookie inaccessible to the JavaScript's document.cookie API.
            options.Secure = CookieSecurePolicy.Always;
        });

        string allowCrossOriginalUrl = configuration.GetSection("AppSettings")["AllowCrossOriginalUrl"];

        if (string.IsNullOrEmpty(allowCrossOriginalUrl))
        {
            throw new ArgumentNullException(nameof(allowCrossOriginalUrl));
        }

        string[] allowedOrigins = allowCrossOriginalUrl.Split(',');

        services.AddCors(options =>
        {
            options
                .AddPolicy(Constants.AllowCrossOriginPolicy, builder =>
                {
                    builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Disposition");
                });
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(90);
            options.Cookie.HttpOnly = true; //cookie is accessible by client-side script
            options.Cookie.IsEssential = true;
        });

        services.AddMvc();

        // Add swagger
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "United Airlines API",
                Description = "Backend API for United Airlines flight request",
            });
        });

        //Registers the API configuration instance which TOptions will bind against
        services.Configure<APIConfig>(configuration.GetSection("APIConfig"));

        //register services
        services.AddScoped<ICargoService, CargoService>();
        return services;
    }

    public static void UseServices(this WebApplication app)
    {
        //adds the Swagger middleware only if the current environment is set to Development
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty; //serves the Swagger UI at the app's root
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(Constants.AllowCrossOriginPolicy);

        app.UseAuthentication();
        app.UseAuthorization();

        //Use this to send status codes as text in the responses
        app.UseStatusCodePages();

        app.UseSession();

        app.UseCookiePolicy();

        //Use this to mitigate the XSS injection vulnerability
        //when the web-application does not properly utilize the
        //"X_FRAME_OPTIONS" header to restrict embedding web-pages inside of a frame
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            await next();
        });

        app.UseEndpoints(endPoints =>
        {
            endPoints.MapControllers();
        });
    }

    public static void ConfigureHttpClientWithCertificate(this IServiceCollection services, 
        string namedClient, IConfiguration configuration)
    {
        try
        {
            var timeout
                = TimeSpan.FromMinutes(Convert.ToInt32(configuration.GetSection("APIConfig")["ServiceTimeoutMin"]));

            services
                .AddHttpClient(namedClient, _ => _.Timeout = timeout)
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    HttpClientHandler handler = new()
                    {
                        ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true
                    };

                    string? certificateDirectory = Environment.GetEnvironmentVariable("CERTIFICATE_DIRECTORY_PATH");

                    if (!string.IsNullOrEmpty(certificateDirectory))
                    {
                        var certificateName = configuration.GetSection("APIConfig")["CertificateName"];
                        var certificatePath = Path.Combine(certificateDirectory, certificateName);

                        if (File.Exists(certificateName))
                        {
                            string certSecret = configuration.GetSection("APIConfig")["CertificatePassword"];
                            var certBytes = File.ReadAllBytes(certificateName);
                            var certificate = new X509Certificate2(certBytes, certSecret);

                            handler.ClientCertificates.Add(certificate);

                            //TODO: Log

                        }

                    }

                    return handler;
                });
        }
        catch (Exception ex)
        {
            //TODO: Log
            throw;
        }
    }
}
