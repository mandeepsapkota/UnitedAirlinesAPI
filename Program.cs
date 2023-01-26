using Microsoft.OpenApi.Models;
using UnitedAirlinesAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//register custom services to middleware
DependencyExtensions.AddServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

//adds the Swagger middleware only if the current environment is set to Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; //serves the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
