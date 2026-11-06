using Infraestructura.Meteorologia;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructura.WeatherAPI
{
    public class ConectorWeatherAPI : IConnectorMeteorologiaAPI
    {
        private const string API_KEY = "cc50f7e9318d4f59b69225451261404";
        private const string API_QUERY = "v1/current.json?key={0}&q={1},{2}&aqi=no";

        private readonly IHttpClientFactory _httpClientFactory;


        public ConectorWeatherAPI(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MeteorologiaDTO> ObtenerDatosMeteorologia(double latitud, double longitud)
        {
            var client = _httpClientFactory.CreateClient("WeatherAPIService");          

            var result = await client.GetAsync(String.Format(API_QUERY, Convert.ToString(latitud, CultureInfo.InvariantCulture), 
                Convert.ToString(longitud, CultureInfo.InvariantCulture)));

            var content = await result.Content.ReadAsStringAsync();

            var resultado = System.Text.Json.JsonSerializer.Deserialize<WeatherDTO>(content);

            if (resultado == null)
            {
                return null;
            }

            return resultado.Convertir();

        }
    }
}
