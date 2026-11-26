using Infraestructura.Lampara;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Negocio.Lampara
{
    public class LamparaService : ILamparaService
    {
        private readonly ILamparaRepository _lamparaRepository;
        private readonly Negocio.Clima.IClimaService _climaService;
        private readonly IConfiguration _configuration;
        private readonly IESP32Client _esp32Client;

        public LamparaService(
            ILamparaRepository lamparaRepository,
            Negocio.Clima.IClimaService climaService,
            IConfiguration configuration,
            IESP32Client esp32Client)
        {
            _lamparaRepository = lamparaRepository;
            _climaService = climaService;
            _configuration = configuration;
            _esp32Client = esp32Client;
        }

        public async Task ProcesarDatosLampara(string json)
        {
            var lamparaDTO = JsonSerializer.Deserialize<LamparaDTO>(json);

            if (lamparaDTO == null)
            {
                throw new System.Exception("No se pudo deserializar el JSON de la lámpara");
            }

            // Obtener datos del clima API
            double temperaturaApi = 0;
            double humedadApi = 0;
            try
            {
                string ubicacion = _configuration["WeatherAPI:Ubicacion"] ?? "Buenos Aires";
                var climaDTO = await _climaService.ObtenerClimaActual(ubicacion);
                temperaturaApi = climaDTO.Temperatura;
                humedadApi = climaDTO.Humedad;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error al obtener clima: {ex.Message}");
            }

            var entity = lamparaDTO.Convertir(temperaturaApi, humedadApi);
            await _lamparaRepository.GuardarLectura(entity);
        }

        public async Task ConsultarESP32Async()
        {
            string json = await _esp32Client.ObtenerDatosLamparaAsync();
            await ProcesarDatosLampara(json);
        }

        public async Task CambiarModoLamparaAsync(int modoId)
        {
            await _esp32Client.CambiarModoLamparaAsync(modoId);
        }

        public async Task<List<DatosLamparaDTO>> ObtenerTodasLecturas()
        {
            var entidades = await _lamparaRepository.ObtenerTodasLecturas();
            return entidades.Select(e => new DatosLamparaDTO
            {
                Id = e.Id,
                FechaHora = e.FechaHora,
                DispositivoId = e.DispositivoId ?? "",
                TemperaturaSensor = e.TemperaturaSensor,
                HumedadSensor = e.HumedadSensor,
                TemperaturaApi = e.TemperaturaApi,
                HumedadApi = e.HumedadApi,
                EstadoLampara = e.EstadoLampara ?? ""
            }).ToList();
        }

        public async Task<DatosLamparaDTO?> ObtenerUltimaLectura()
        {
            var entidad = await _lamparaRepository.ObtenerUltimaLectura();
            
            if (entidad == null)
            {
                return null;
            }

            return new DatosLamparaDTO
            {
                Id = entidad.Id,
                FechaHora = entidad.FechaHora,
                DispositivoId = entidad.DispositivoId ?? "",
                TemperaturaSensor = entidad.TemperaturaSensor,
                HumedadSensor = entidad.HumedadSensor,
                TemperaturaApi = entidad.TemperaturaApi,
                HumedadApi = entidad.HumedadApi,
                EstadoLampara = entidad.EstadoLampara ?? ""
            };
        }
    }
}
