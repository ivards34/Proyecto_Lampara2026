namespace AppLamparaLed.Models;

public class LecturaHistorica
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string DispositivoId { get; set; } = string.Empty;
    public double TemperaturaSensor { get; set; }
    public double HumedadSensor { get; set; }
    public double TemperaturaApi { get; set; }
    public double HumedadApi { get; set; }
    public string EstadoLampara { get; set; } = string.Empty;
}
