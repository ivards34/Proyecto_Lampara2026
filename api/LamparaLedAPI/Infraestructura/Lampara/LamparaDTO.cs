using System.Text.Json.Serialization;

namespace Infraestructura.Lampara
{
    public class LamparaDTO
    {
        [JsonPropertyName("fecha_hora")]
        public string? FechaHora { get; set; }

        [JsonPropertyName("dispositivo")]
        public string? DispositivoId { get; set; }

        [JsonPropertyName("temperatura")]
        public double TemperaturaSensor { get; set; }

        [JsonPropertyName("humedad")]
        public double HumedadSensor { get; set; }

        [JsonPropertyName("lampara")]
        public string? EstadoLampara { get; set; }

        public LamparaEntity Convertir(double temperaturaApi = 0, double humedadApi = 0)
        {
            DateTime fechaParseada;
            if (DateTime.TryParse(FechaHora, out fechaParseada))
            {
                if (fechaParseada.Kind == DateTimeKind.Unspecified)
                {
                    fechaParseada = DateTime.SpecifyKind(fechaParseada, DateTimeKind.Local);
                }
            }
            else
            {
                fechaParseada = DateTime.Now;
            }

            return new LamparaEntity
            {
                FechaHora = fechaParseada,
                DispositivoId = DispositivoId ?? "",
                TemperaturaSensor = TemperaturaSensor,
                HumedadSensor = HumedadSensor,
                TemperaturaApi = temperaturaApi,
                HumedadApi = humedadApi,
                EstadoLampara = EstadoLampara ?? ""
            };
        }
    }
}
