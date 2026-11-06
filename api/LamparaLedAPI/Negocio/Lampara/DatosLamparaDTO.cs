namespace Negocio.Lampara
{
    public class DatosLamparaDTO
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string? DispositivoId { get; set; }
        public double TemperaturaSensor { get; set; }
        public double HumedadSensor { get; set; }
        public double TemperaturaApi { get; set; }
        public double HumedadApi { get; set; }
        public string? EstadoLampara { get; set; }
    }
}
