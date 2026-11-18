using System.Text;
using System.Text.Json;
using AppLamparaLed.Models;

namespace AppLamparaLed.Services;

public class LamparaService : ILamparaService
{
    private readonly HttpClient _httpClient;
    private string ApiUrl => AppSettings.ApiBaseUrl;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LamparaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<bool> ConsultarEsp32Async()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Conectando a API: {ApiUrl}/consultar");
            var response = await _httpClient.GetAsync($"{ApiUrl}/consultar");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Respuesta API: {result}");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error consultando ESP32: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CambiarModoAsync(int modoId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Cambiando modo via API: {ApiUrl}/modo, modoId={modoId}");
            var body = new { modoId };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ApiUrl}/modo", content);
            System.Diagnostics.Debug.WriteLine($"Response status: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Respuesta cambio modo: {result}");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cambiando modo: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<List<LecturaHistorica>> ObtenerTodasLasLecturasAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Obteniendo lecturas de API: {ApiUrl}/lecturas");
            var response = await _httpClient.GetAsync($"{ApiUrl}/lecturas");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Lecturas obtenidas: {json.Length} caracteres");
            System.Diagnostics.Debug.WriteLine($"JSON: {json}");
            var result = JsonSerializer.Deserialize<List<LecturaHistorica>>(json, _jsonOptions) ?? new List<LecturaHistorica>();
            System.Diagnostics.Debug.WriteLine($"Deserialización: {result.Count} lecturas");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error obteniendo lecturas: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<LecturaHistorica>();
        }
    }

    public async Task<LecturaHistorica?> ObtenerDatosActualesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Obteniendo datos actuales de API: {ApiUrl}/actual");
            var response = await _httpClient.GetAsync($"{ApiUrl}/actual");
            System.Diagnostics.Debug.WriteLine($"Response status: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Datos actuales obtenidos: {json.Length} caracteres");
            System.Diagnostics.Debug.WriteLine($"JSON completo: {json}");
            var result = JsonSerializer.Deserialize<LecturaHistorica>(json, _jsonOptions);
            System.Diagnostics.Debug.WriteLine($"Deserialización exitosa: {result?.FechaHora}");
            if (result != null)
            {
                System.Diagnostics.Debug.WriteLine($"Valores deserializados: TempSensor={result.TemperaturaSensor}, HumSensor={result.HumedadSensor}, TempApi={result.TemperaturaApi}, HumApi={result.HumedadApi}, Estado={result.EstadoLampara}");
            }
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error obteniendo datos actuales: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }
}
