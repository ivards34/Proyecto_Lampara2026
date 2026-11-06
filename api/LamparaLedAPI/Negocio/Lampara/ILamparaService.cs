using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Negocio.Lampara
{
    public interface ILamparaService
    {
        Task ProcesarDatosLampara(string json);
        Task ConsultarESP32Async();
        Task CambiarModoLamparaAsync(int modoId);
        Task<List<DatosLamparaDTO>> ObtenerTodasLecturas();
        Task<DatosLamparaDTO?> ObtenerUltimaLectura();
    }
}
