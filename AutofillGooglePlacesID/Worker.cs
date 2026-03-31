using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Services;
using Models;

namespace AutofillGooglePlacesID
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _randomizer;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
            _randomizer = new Random();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Inciando el Worker Service de pruebas...");

            // El ciclo infinito que mantiene vivo al Worker hasta que apagues el programa
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // =====================================================================
                    // MODO ACTUAL: API DUMMYJSON (Pruebas gratuitas)
                    // =====================================================================
                    
                    // 1. GENERAMOS UN DATO ALEATORIO (Para simular búsquedas distintas)
                    int randomId = _randomizer.Next(1, 30); // DummyJSON/products 
                    _logger.LogInformation("Buscando información para el ID ficticio: {Id} a las {Time}", randomId, DateTimeOffset.Now);

                    var httpClient = _httpClientFactory.CreateClient(); 
                    string dummyUrl = $"https://dummyjson.com/products/{randomId}";
                    HttpResponseMessage response = await httpClient.GetAsync(dummyUrl, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);

                        // Parseamos el JSON para extraer solo el título y el precio
                        using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                        JsonElement root = doc.RootElement;
                        string productName = root.GetProperty("title").GetString() ?? "Sin nombre";
                        string productDesc = root.GetProperty("description").GetString() ?? "Sin descripción";
                        string productCat = root.GetProperty("category").GetString() ?? "Sin categoría";
                        double price = root.GetProperty("price").GetDouble();

                        _logger.LogInformation("Éxito! Producto encontrado: {Name}, {Cat}, Descripción:{Desc} - Precio: ${Price}", productName, productCat, productDesc, price);
                    }
                    else
                    {
                        _logger.LogWarning("Error en DummyJSON: {StatusCode}", response.StatusCode);
                    }

                    // =====================================================================
                    // MODO FUTURO: GOOGLE PLACES API (NEW) + ENTITY FRAMEWORK
                    // Descomentar esto cuando se resuelva el pago en Google Cloud
                    // =====================================================================
                    /*
                    // 1. ABRIR CONEXIÓN A LA BASE DE DATOS
                    // Como el Worker es Singleton (vive siempre), usamos _scopeFactory para crear 
                    // una instancia temporal de GeoConnectContext y evitar choques en la memoria.
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<GeoConnectContext>();

                    // 2. BUSCAR LUGARES PENDIENTES
                    // Traemos de la base de datos solo aquellos lugares que no tengan el GooglePlaceId.
                    var lugaresPendientes = await dbContext.Lugares
                        .Where(l => string.IsNullOrEmpty(l.GooglePlaceId))
                        .ToListAsync(stoppingToken);

                    if (lugaresPendientes.Any())
                    {
                        _logger.LogInformation("Se encontraron {Count} lugares pendientes por procesar.", lugaresPendientes.Count);
                        
                        string apiKey = _configuration["GooglePlaces:ApiKey"]; //esto se crea con secretos desde la consola
                        string googleUrl = "https://places.googleapis.com/v1/places:searchText";

                        foreach (var lugar in lugaresPendientes)
                        {
                            // 3. ARMAR LA PETICIÓN A GOOGLE PLACES
                            // Usamos el nombre del lugar de tu BD para buscarlo en Google (Ej: "Pueblito Paisa")
                            string query = $"{lugar.NombreLugar}"; 
                            var requestBody = new { textQuery = query };
                            string jsonBody = JsonSerializer.Serialize(requestBody);
                            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                            var request = new HttpRequestMessage(HttpMethod.Post, googleUrl) { Content = content };
                            request.Headers.Add("X-Goog-Api-Key", apiKey);
                            // FieldMask: Le decimos a Google que SOLO queremos el ID y las coordenadas (Ahorra dinero de la cuota)
                            request.Headers.Add("X-Goog-FieldMask", "places.id,places.location");

                            HttpResponseMessage googleResponse = await httpClient.SendAsync(request, stoppingToken);

                            if (googleResponse.IsSuccessStatusCode)
                            {
                                // 4. PROCESAR RESPUESTA Y ACTUALIZAR EL OBJETO
                                string googleJson = await googleResponse.Content.ReadAsStringAsync(stoppingToken);
                                using JsonDocument doc = JsonDocument.Parse(googleJson);
                                JsonElement rootNode = doc.RootElement;

                                if (rootNode.TryGetProperty("places", out JsonElement places) && places.GetArrayLength() > 0)
                                {
                                    JsonElement firstPlace = places[0];
                                    
                                    // Asignamos los datos de Google a nuestra entidad de base de datos
                                    lugar.GooglePlaceId = firstPlace.GetProperty("id").GetString();
                                    
                                    if (firstPlace.TryGetProperty("location", out JsonElement location))
                                    {
                                        double lat = location.GetProperty("latitude").GetDouble();
                                        double long = location.GetProperty("longitude").GetDouble();

                                        //NetTopology usa (Longitud, Latitud) => (X, Y)
                                        //SRID 4326 sistema de referencia estándar internacional (wgs84)

                                        lugar.Coordenadas = new Point (long, lat) { SRID = 4326 };
                                    }

                                    _logger.LogInformation("Actualizado: {Lugar} -> PlaceId: {PlaceId}", lugar.Nombre, lugar.GooglePlaceId);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Error consultando {Lugar}: {StatusCode}", lugar.Nombre, googleResponse.StatusCode);
                            }

                            // Pausa de 3/4 de segundo entre peticiones para no hacerle spam a la API de Google
                            await Task.Delay(750, stoppingToken);
                        }

                        // 5. GUARDAR CAMBIOS EN LA BASE DE DATOS
                        // Guardamos todas las actualizaciones de una sola vez
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Base de datos actualizada con éxito.");
                    }
                    else
                    {
                        _logger.LogInformation("No hay lugares pendientes de actualización.");
                    }
                    */
                }
                catch (Exception ex)
                {
                    // Evitamos que un error tumbe todo el servicio
                    _logger.LogError("Ocurrió un error inesperado en el ciclo: {Message}", ex.Message);
                }

                // 3. DESCANSO DEL WORKER
                // Espera 10 segundos antes de volver a empezar el ciclo.
                // En producción, esto será probablemente 24 horas (Task.Delay(TimeSpan.FromHours(24)))
                _logger.LogInformation("Worker durmiendo por 5 segundos...\n");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}