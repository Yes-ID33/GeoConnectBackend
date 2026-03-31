using Services;
using AutofillGooglePlacesID;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Configuración de Base de Datos
builder.Services.AddDbContext<GeoConnectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.UseNetTopologySuite()));

// Inyección del Factory para HTTP
builder.Services.AddHttpClient();

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