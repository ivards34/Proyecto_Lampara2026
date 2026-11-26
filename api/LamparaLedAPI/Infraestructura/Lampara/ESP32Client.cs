using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Infraestructura.Lampara
{
    public class ESP32Client : IESP32Client
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ESP32Client(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ESP32:BaseUrl"];
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                throw new InvalidOperationException("Configure ESP32:BaseUrl en User Secrets o mediante una variable de entorno.");
            }
        }

        public async Task<string> ObtenerDatosLamparaAsync()
        {
            string url = $"{_baseUrl}/api/v1/lampara";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task CambiarModoLamparaAsync(int modoId)
        {
            string url = $"{_baseUrl}/api/v1/lampara/modo?id={modoId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
