using System.Threading.Tasks;

namespace Negocio.Clima
{
    public interface IMeteorologia
    {
        public Task<DatosMeteorologiaDTO> ObtenerMeteorologiaActual(double latitud, double longitud);
    }
}
