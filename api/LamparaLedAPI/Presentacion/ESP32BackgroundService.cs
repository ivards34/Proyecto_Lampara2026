using Infraestructura.Lampara;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Negocio.Lampara;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presentacion
{
    public class ESP32BackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _intervalo;

        public ESP32BackgroundService(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            
            int minutos = int.Parse(_configuration["ESP32:IntervaloMinutos"] ?? "30");
            _intervalo = TimeSpan.FromMinutes(minutos);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now}] Consultando ESP32...");
                    
                    // Este servicio es Singleton (una sola instancia para toda la app),
                    // pero ILamparaService está registrado como Scoped. Por eso en cada
                    // iteración se crea un scope nuevo: el contenedor de DI resuelve ahí
                    // LamparaService con sus dependencias (LamparaDbContext, IClimaService,
                    // IESP32Client) y las descarta al terminar el using, igual que si fuera
                    // un request HTTP.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var lamparaService = scope.ServiceProvider.GetRequiredService<ILamparaService>();
                        await lamparaService.ConsultarESP32Async();
                    }
                    
                    Console.WriteLine($"[{DateTime.Now}] Datos guardados correctamente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now}] Error al consultar ESP32: {ex.Message}");
                }

                await Task.Delay(_intervalo, stoppingToken);
            }
        }
    }
}
