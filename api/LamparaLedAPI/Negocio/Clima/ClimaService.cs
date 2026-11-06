using Infraestructura.WeatherAPI;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

namespace Negocio.Clima
{
    public class ClimaService : IClimaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ClimaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherAPI:ApiKey"] ?? "";
        }

        public async Task<ClimaDTO> ObtenerClimaActual(string ubicacion)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new System.Exception("La API key de WeatherAPI no está configurada");
            }

            string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={ubicacion}&aqi=no";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<WeatherAPIResponse>(content);

            if (weatherResponse?.Current == null)
            {
                throw new System.Exception("No se pudo obtener los datos del clima");
            }

            return new ClimaDTO
            {
                Temperatura = weatherResponse.Current.TempC,
                Humedad = weatherResponse.Current.Humidity
            };
        }
    }
}
