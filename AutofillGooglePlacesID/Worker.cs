using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutofillGooglePlacesID
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly Random _randomizer;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
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
                    // 1. GENERAMOS UN DATO ALEATORIO (Para simular búsquedas distintas)
                    int randomId = _randomizer.Next(1, 30); // DummyJSON/products 
                    _logger.LogInformation("Buscando información para el ID ficticio: {Id} a las {Time}", randomId, DateTimeOffset.Now);

                    // =====================================================================
                    // MODO ACTUAL: API DUMMYJSON (Pruebas gratuitas)
                    // =====================================================================
                    string dummyUrl = $"https://dummyjson.com/products/{randomId}";
                    HttpResponseMessage response = await _httpClient.GetAsync(dummyUrl, stoppingToken);

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
                    // MODO FUTURO: GOOGLE PLACES API (NEW)
                    // Descomentar esto cuando se apruebe el presupuesto y borrar lo de arriba
                    // =====================================================================
                    /*
                    string apiKey = _configuration["Google:PlacesApiKey"];
                    string googleUrl = "https://places.googleapis.com/v1/places:searchText";

                    // La nueva API exige decirle qué vamos a buscar en el Body (POST)
                    var requestBody = new { textQuery = "Restaurantes en Medellín" }; // Aquí luego pondremos lugares de tu BD
                    string jsonBody = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    // Configuramos la petición agregando los Headers obligatorios
                    var request = new HttpRequestMessage(HttpMethod.Post, googleUrl);
                    request.Content = content;
                    request.Headers.Add("X-Goog-Api-Key", apiKey);
                    
                    // IMPORTANTE: FieldMask para que Google no cobre datos innecesarios
                    request.Headers.Add("X-Goog-FieldMask", "places.id,places.displayName,places.formattedAddress");

                    HttpResponseMessage googleResponse = await _httpClient.SendAsync(request, stoppingToken);

                    if (googleResponse.IsSuccessStatusCode)
                    {
                        string googleJson = await googleResponse.Content.ReadAsStringAsync(stoppingToken);
                        _logger.LogInformation("Respuesta de Google Places recibida.");
                        // Aquí procesaremos el JSON de Google para guardarlo en tu BD
                    }
                    else
                    {
                        string errorMsg = await googleResponse.Content.ReadAsStringAsync(stoppingToken);
                        _logger.LogError("Error en Google Places: {Error}", errorMsg);
                    }
                    */
                    // =====================================================================
                }
                catch (Exception ex)
                {
                    // Evitamos que un error tumbe todo el servicio
                    _logger.LogError("Ocurrió un error inesperado en el ciclo: {Message}", ex.Message);
                }

                // 3. DESCANSO DEL WORKER
                // Espera 10 segundos antes de volver a empezar el ciclo.
                // En producción, esto será probablemente 24 horas (Task.Delay(TimeSpan.FromHours(24)))
                _logger.LogInformation("Worker durmiendo por 30 segundos...\n");
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}