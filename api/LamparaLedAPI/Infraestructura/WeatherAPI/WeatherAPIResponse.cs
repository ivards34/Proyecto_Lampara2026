using System.Text.Json.Serialization;

namespace Infraestructura.WeatherAPI
{
    public class WeatherAPIResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }
    }
}
