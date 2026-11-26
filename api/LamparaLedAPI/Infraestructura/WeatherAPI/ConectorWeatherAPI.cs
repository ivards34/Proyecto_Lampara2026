using Infraestructura.Meteorologia;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructura.WeatherAPI
{
    public class ConectorWeatherAPI : IConnectorMeteorologiaAPI
    {
        private const string API_QUERY = "v1/current.json?key={0}&q={1}&aqi=no";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public ConectorWeatherAPI(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["WeatherAPI:ApiKey"] ?? "";
        }

        public async Task<MeteorologiaDTO> ObtenerDatosMeteorologia(string consulta)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new System.Exception("La API key de WeatherAPI no está configurada");
            }

            var client = _httpClientFactory.CreateClient("WeatherAPIService");

            string query = string.Format(API_QUERY, _apiKey, Uri.EscapeDataString(consulta));

            var response = await client.GetAsync(query);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<WeatherDTO>(content);

            return resultado?.Convertir();
        }
    }
}
