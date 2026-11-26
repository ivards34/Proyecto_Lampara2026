using AppLamparaLed.Models;

namespace AppLamparaLed.Services;

public interface ILamparaService
{
    Task<bool> ConsultarEsp32Async();
    Task<bool> CambiarModoAsync(int modoId);
    Task<List<LecturaHistorica>> ObtenerTodasLasLecturasAsync();
    Task<LecturaHistorica?> ObtenerDatosActualesAsync();
}
