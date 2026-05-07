using Models;
using Services;
using Services.Interface;
using AutofillGooglePlacesID;
using Microsoft.EntityFrameworkCore;
using Services.Implements;

var builder = Host.CreateApplicationBuilder(args);

// Configuración de Base de Datos
builder.Services.AddDbContext<GeoConnectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB"),
    x => x.UseNetTopologySuite()));

// Inyección del Factory para HTTP
builder.Services.AddHttpClient();

builder.Services.AddScoped<IEmailService, EmailService>();

// Registro del Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

//Bloque de seeding
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GeoConnectContext>();
    DataSeeder.Seed(context);
}
host.Run();