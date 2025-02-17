using Microsoft.EntityFrameworkCore;
using MyBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// // Lägg till CORS-tjänsten
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Tillåt Angular
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true) // Om du vill tillåta flera origins
              .AllowCredentials(); // Om du använder cookies/autentisering
    });
});

// Lägg till databaskonfiguration (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

// Aktivera CORS innan API-rutterna
app.UseCors("AllowAngular");

// Aktivera routing och API controllers
app.UseRouting();
// app.UseAuthorization();

app.MapControllers();

app.Run();
