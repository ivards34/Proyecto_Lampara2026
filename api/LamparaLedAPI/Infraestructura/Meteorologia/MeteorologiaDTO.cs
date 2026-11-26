namespace Infraestructura.Meteorologia
{
    public class MeteorologiaDTO
    {
        public float TemperaturaCelsius { get; set; }
        public float TemperaturaFahrenheit { get; set; }
        public int Humedad { get; set; }
        public float Presion { get; set; }
        public float VelocidadVientoKph { get; set; }
        public float VelocidadVientoMph { get; set; }
        public string DireccionViento { get; set; } = "";
        public int GradosViento { get; set; }
        public string CondicionDescripcion { get; set; } = "";
        public float Latitud { get; set; }
        public float Longitud { get; set; }
    }
}
