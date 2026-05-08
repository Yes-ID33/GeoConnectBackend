using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Globalization;
using Services;
using Models;
using Services.Interface;

namespace AutofillGooglePlacesID
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando el Worker Service con Nominatim (OpenStreetMap)...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<GeoConnectContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    // 1. BUSCAR LUGARES PENDIENTES
                    // Traemos los lugares que NO tienen coordenadas asignadas
                    var lugaresPendientes = await dbContext.Lugares
                        .Include(l => l.Municipio)
                        .Where(l => l.Coordenadas == null || l.FotoUrl == null)
                        .ToListAsync(stoppingToken);

                    if (lugaresPendientes.Any())
                    {
                        _logger.LogInformation("Se encontraron {Count} lugares pendientes por geocodificar/fotografiar.", lugaresPendientes.Count);

                        var httpClient = _httpClientFactory.CreateClient();
                        var lugaresFallidos = new List<string>();

                        // 1. INICIAMOS EL CONTADOR DE UNSPLASH
                        int contadorUnsplash = 0;
                        bool limiteUnsplashAlcanzado = false;

                        foreach (var lugar in lugaresPendientes)
                        {
                            // Detener si el servicio se está apagando
                            stoppingToken.ThrowIfCancellationRequested();

                            try
                            {
                                // --- PRIMERA PARTE: COORDENADAS (Nominatim) ---
                                if (lugar.Coordenadas == null)
                                {
                                    // Armamos las estrategias de búsqueda en cascada
                                    var intentos = new List<string>
                                    {
                                        // Intento 1: Nombre + Municipio (Ideal para turismo urbano)
                                        $"{lugar.NombreLugar}, {lugar.Municipio?.NombreMunicipio}, Antioquia, Colombia",
                    
                                        // Intento 2: Nombre + Departamento (Ideal para lugares rurales/naturales)
                                        $"{lugar.NombreLugar}, Antioquia, Colombia"
                                    };

                                    // Intento 3: Dirección Urbana + Municipio (Solo si tiene dirección válida)
                                    if (!string.IsNullOrEmpty(lugar.Direccion) &&
                                       (lugar.Direccion.StartsWith("Calle", StringComparison.OrdinalIgnoreCase) ||
                                        lugar.Direccion.StartsWith("Carrera", StringComparison.OrdinalIgnoreCase) ||
                                        lugar.Direccion.StartsWith("Cra", StringComparison.OrdinalIgnoreCase) ||
                                        lugar.Direccion.StartsWith("Cl", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        intentos.Add($"{lugar.Direccion}, {lugar.Municipio?.NombreMunicipio}, Antioquia, Colombia");
                                    }

                                    bool coordenadaEncontrada = false;

                                    // Empezamos a disparar las búsquedas una por una
                                    foreach (var terminoBusqueda in intentos)
                                    {
                                        string query = Uri.EscapeDataString(terminoBusqueda);
                                        string nominatimUrl = $"https://nominatim.openstreetmap.org/search?q={query}&format=jsonv2&limit=1";

                                        var request = new HttpRequestMessage(HttpMethod.Get, nominatimUrl);
                                        request.Headers.Add("User-Agent", "GeoConnectApp/1.0 (yesid.maldonado820@pascualbravo.edu.co)");

                                        HttpResponseMessage response = await httpClient.SendAsync(request, stoppingToken);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);
                                            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                                            JsonElement root = doc.RootElement;

                                            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                                            {
                                                JsonElement firstResult = root[0];
                                                string? latString = firstResult.GetProperty("lat").GetString();
                                                string? lonString = firstResult.GetProperty("lon").GetString();

                                                if (double.TryParse(latString, NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                                                    double.TryParse(lonString, NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                                                {
                                                    lugar.Coordenadas = new Point(lon, lat) { SRID = 4326 };
                                                    _logger.LogInformation("Éxito Coordenadas: {Lugar} -> {{Lat: {Lat}, Lon: {Lon}}} (Con: {Termino})",
                                                        lugar.NombreLugar, lat, lon, terminoBusqueda);
                                                    coordenadaEncontrada = true;
                                                    await Task.Delay(1100, stoppingToken);
                                                    break; // ¡Acertamos! Salimos del foreach de intentos para no buscar más.
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Error en intento Nominatim para {Lugar}: {StatusCode}", lugar.NombreLugar, response.StatusCode);
                                        }

                                        // Retraso estricto de Nominatim (1.1s) POR CADA INTENTO
                                        await Task.Delay(1100, stoppingToken);
                                    }

                                    // Si terminó el ciclo de intentos y no encontró nada, lo marcamos como fallido
                                    if (!coordenadaEncontrada)
                                    {
                                        _logger.LogWarning("Nominatim agotó todos los intentos para: {Lugar}", lugar.NombreLugar);
                                        lugaresFallidos.Add($"{lugar.NombreLugar} (Sin coordenadas) en {lugar.Municipio?.NombreMunicipio}");
                                    }
                                }

                                // --- SEGUNDA PARTE: FOTOS (Unsplash) ---
                                if (string.IsNullOrEmpty(lugar.FotoUrl))
                                {
                                    // 2. VERIFICAMOS EL LÍMITE ANTES DE CONSULTAR
                                    if (contadorUnsplash < 45)
                                    {
                                        contadorUnsplash++; // Sumamos 1 al contador
                                        string? urlEncontrada = await BuscarFotoAsync(httpClient, lugar.NombreLugar, lugar.Municipio?.NombreMunicipio, stoppingToken);

                                        if (!string.IsNullOrEmpty(urlEncontrada))
                                        {
                                            lugar.FotoUrl = urlEncontrada;
                                            _logger.LogInformation("Foto encontrada para {Lugar} (Petición Unsplash #{Contador})", lugar.NombreLugar, contadorUnsplash);
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Unsplash no encontró foto para: {Lugar}", lugar.NombreLugar);
                                        }
                                    }
                                    else
                                    {
                                        // 3. SI LLEGAMOS A 45, DEJAMOS DE BUSCAR FOTOS EN ESTE CICLO
                                        if (!limiteUnsplashAlcanzado)
                                        {
                                            _logger.LogWarning("¡Límite de seguridad de Unsplash alcanzado (45)! Las fotos restantes se buscarán en el próximo ciclo (dentro de 2 horas).");
                                            limiteUnsplashAlcanzado = true; // Para no repetir este mensaje en cada iteración
                                        }
                                    }
                                }
                            }
                            catch (Exception exLugar)
                            {
                                _logger.LogError("Error aislado procesando {Lugar}: {Error}", lugar.NombreLugar, exLugar.Message);
                                lugaresFallidos.Add($"{lugar.NombreLugar} - Error interno: {exLugar.Message}");
                            }
                        }

                        // 4. GUARDAR CAMBIOS EN LA BASE DE DATOS
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Geocodificación terminada. Base de datos actualizada.");

                        if (lugaresFallidos.Any())
                        {
                            _logger.LogInformation("Enviando reporte de {Count} fallos a los desarrolladores...", lugaresFallidos.Count);
                            try
                            {
                                var emailSettings = _configuration.GetSection("EmailSettings");
                                var dev1 = emailSettings["Dev1Email"]!;
                                var dev2 = emailSettings["Dev2Email"]!;
                                var dev3 = emailSettings["Dev3Email"]!;
                                await emailService.EnviarReporteConsumoCoordsAsync(dev1, lugaresFallidos);
                                _logger.LogInformation("Correo de reporte enviado exitosamente al Dev1.");
                                await emailService.EnviarReporteConsumoCoordsAsync(dev2, lugaresFallidos);
                                _logger.LogInformation("Correo de reporte enviado exitosamente al Dev2.");
                                await emailService.EnviarReporteConsumoCoordsAsync(dev3, lugaresFallidos);
                                _logger.LogInformation("Correo de reporte enviado exitosamente al Dev3.");
                            }
                            catch (Exception exMail)
                            {
                                _logger.LogError("No se pudo enviar el correo de reporte: {Message}", exMail.Message);
                            }
                        }
                    }    
                    else
                    {
                        _logger.LogInformation("No hay lugares pendientes de actualización de coordenadas.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ocurrió un error inesperado en el ciclo: {Message}", ex.Message);
                }

                // 5. DESCANSO DEL WORKER
                // Si ya procesó todo o no había nada, duerme 2 horas antes de volver a verificar la DB.
                _logger.LogInformation("Worker durmiendo por 2 horas...\n");
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }

        private async Task<string?> BuscarFotoAsync(HttpClient httpClient, string nombreLugar, string? nombreMunicipio, CancellationToken stoppingToken)
        {
            try
            {
                // 1. CONSTRUIR LA BÚSQUEDA
                // Unsplash busca en inglés y español, pero es más preciso si le damos contexto.
                // Ejemplo resultado: "Parque Principal Concepción Colombia"
                string terminoBusqueda = $"{nombreLugar} {nombreMunicipio} Colombia";

                // Reemplaza los espacios con "%20" para que internet no se confunda
                string query = Uri.EscapeDataString(terminoBusqueda);

                // 2. CONFIGURAR LA URL Y LA API KEY
                // RECUERDA: Tienes que crear una cuenta en unsplash.com/developers y cambiar este texto por tu Key
                string clientId = _configuration["UnsplashSettings:AccessKey"]!;
                string unsplashUrl = $"https://api.unsplash.com/search/photos?page=1&query={query}&client_id={clientId}&per_page=1";

                // 3. HACER LA PETICIÓN
                var request = new HttpRequestMessage(HttpMethod.Get, unsplashUrl);
                // Unsplash pide por regla que le digamos qué versión de su API usamos
                request.Headers.Add("Accept-Version", "v1");

                var response = await httpClient.SendAsync(request, stoppingToken);

                // 4. LEER LA RESPUESTA
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);
                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);

                    // Navegamos por el JSON: buscamos la propiedad "results" que es un Array (lista)
                    var results = doc.RootElement.GetProperty("results");

                    // Si la lista es mayor a 0, significa que Unsplash encontró al menos una foto
                    if (results.GetArrayLength() > 0)
                    {
                        // results[0] -> El primer resultado
                        // .GetProperty("urls") -> Entramos a la sección de enlaces
                        // .GetProperty("regular") -> Elegimos el tamaño "regular" (aprox 1080px)
                        // .GetString() -> Lo convertimos a texto puro de C#
                        return results[0].GetProperty("urls").GetProperty("regular").GetString();
                    }
                }
            }
            catch (Exception)
            {
                // Si se cae el internet o falla el JSON, lo atrapamos en silencio.
                // No queremos que un error de Unsplash apague todo el Worker.
                // Retornará null al final.
            }

            return null; // Si algo falló o no hubo resultados, devolvemos nulo.
        }
    }
}