using Infraestructura.Meteorologia;
using System.Threading.Tasks;

namespace Negocio.Clima
{
    public class ClimaService : IClimaService
    {
        private readonly IConnectorMeteorologiaAPI _conectorMeteorologia;

        public ClimaService(IConnectorMeteorologiaAPI conectorMeteorologia)
        {
            _conectorMeteorologia = conectorMeteorologia;
        }

        public async Task<ClimaDTO> ObtenerClimaActual(string ubicacion)
        {
            var meteorologiaDTO = await _conectorMeteorologia.ObtenerDatosMeteorologia(ubicacion);

            if (meteorologiaDTO == null)
            {
                throw new System.Exception("No se pudo obtener los datos del clima");
            }

            return new ClimaDTO
            {
                Temperatura = meteorologiaDTO.TemperaturaCelsius,
                Humedad = meteorologiaDTO.Humedad
            };
        }
    }
}
