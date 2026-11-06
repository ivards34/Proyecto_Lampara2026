namespace AppLamparaLed.Models;

public class LamparaData
{
    public string FechaHora { get; set; } = string.Empty;
    public string Dispositivo { get; set; } = string.Empty;
    public double Temperatura { get; set; }
    public double Humedad { get; set; }
    public string Lampara { get; set; } = string.Empty;
}
