using Microsoft.EntityFrameworkCore;
using Models; // O el namespace que tenga tu GeoConnectContext
using Services;
using Services.Implements;
using Services.Interface;
using System.Reflection;

// Nota: En .NET moderno (6, 8, 9) ya no se usa el "public class Program { public static void Main... }"
// Se usan instrucciones de nivel superior (Top-level statements) como en tu versión original.
var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE LA BASE DE DATOS (Tu versión) ---
// Usamos tu cadena de conexión y mantenemos la configuración espacial esencial para el mapa
builder.Services.AddDbContext<GeoConnectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB"),
    x => x.UseNetTopologySuite())
);

// --- 2. INYECCIÓN DE DEPENDENCIAS (Versión del profesor)
// Aquí le decimos a la API: "Cuando un controlador te pida un IUsuarioService, entrégale un UsuarioService".
builder.Services.AddTransient<IAccionLugarService, AccionLugarService>();
builder.Services.AddTransient<IComentarioService, ComentarioService>();
builder.Services.AddTransient<ILugarService, LugarService>();
builder.Services.AddTransient<IMunicipioService, MunicipioService>();
builder.Services.AddTransient<IUsuarioService, UsuarioService>();

builder.Services.AddControllers();

// --- 3. CONFIGURACIÓN DE SWAGGER ---
builder.Services.AddEndpointsApiExplorer();

// Usamos tu lógica para el archivo XML. Es mucho mejor buscarlo por el nombre del ensamblado
// que quemar el nombre "api.xml" como lo hizo el profe, así evitas errores si cambias el nombre del proyecto.
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// --- 4. REDIRECCIÓN A SWAGGER
// Mantenemos tu solución asíncrona para que al darle "Play" abra Swagger de una, 
// sin que Swagger detecte el "/" como un endpoint de tu API.
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html", false);
        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();