using System.Threading.Tasks;

namespace Infraestructura.Lampara
{
    public interface IESP32Client
    {
        Task<string> ObtenerDatosLamparaAsync();
        Task CambiarModoLamparaAsync(int modoId);
    }
}
