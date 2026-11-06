using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AppLamparaLed.Services;
using AppLamparaLed.Utilities;
using AppLamparaLed.Models;

namespace AppLamparaLed.PageModels;

public class MiLamparaPageModel : INotifyPropertyChanged
{
    private readonly ILamparaService _lamparaService;
    private string _selectedState = "Apagado";
    private double _ambientTemperature = 0;
    private double _ambientHumidity = 0;
    private double _climateTemperature = 0;
    private double _climateHumidity = 0;
    private bool _isLoading = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MiLamparaPageModel()
    {
        _lamparaService = new LamparaService();
    }

    public string SelectedState
    {
        get => _selectedState;
        set
        {
            _selectedState = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsFullSelected));
            OnPropertyChanged(nameof(IsLuzTenueSelected));
            OnPropertyChanged(nameof(IsApagadoSelected));
            OnPropertyChanged(nameof(IsLamparaTemperaturaSelected));
        }
    }

    public bool IsFullSelected => SelectedState == "Full";
    public bool IsLuzTenueSelected => SelectedState == "Luz Tenue";
    public bool IsApagadoSelected => SelectedState == "Apagado";
    public bool IsLamparaTemperaturaSelected => SelectedState == "Lámpara Temperatura";

    public double AmbientTemperature
    {
        get => _ambientTemperature;
        set
        {
            _ambientTemperature = value;
            OnPropertyChanged();
        }
    }

    public double AmbientHumidity
    {
        get => _ambientHumidity;
        set
        {
            _ambientHumidity = value;
            OnPropertyChanged();
        }
    }

    public double ClimateTemperature
    {
        get => _climateTemperature;
        set
        {
            _climateTemperature = value;
            OnPropertyChanged();
        }
    }

    public double ClimateHumidity
    {
        get => _climateHumidity;
        set
        {
            _climateHumidity = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public List<string> LampStates => new List<string> { "Full", "Luz Tenue", "Apagado", "Lámpara Temperatura" };

    public ICommand SelectStateCommand => new Command<string>(async (state) => await SelectStateAsync(state));
    public ICommand NavigateToHistoricoCommand => new Command(async () =>
    {
        await Shell.Current.GoToAsync("//historico");
    });
    public ICommand RefreshCommand => new Command(async () =>
    {
        try
        {
            await RefreshDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en RefreshCommand: {ex.Message}");
        }
    });

    private async Task SelectStateAsync(string state)
    {
        if (IsLoading) return;

        IsLoading = true;
        SelectedState = state;

        int modoId = state switch
        {
            "Full" => 1,
            "Luz Tenue" => 2,
            "Lámpara Temperatura" => 3,
            "Apagado" => 4,
            _ => 4
        };

        var success = await _lamparaService.CambiarModoAsync(modoId);

        if (!success)
        {
            await Shell.Current.DisplayAlertAsync("Error", "No se pudo cambiar el modo de la lámpara", "OK");
            IsLoading = false;
            return;
        }

        System.Diagnostics.Debug.WriteLine("Modo cambiado exitosamente, consultando ESP32 para actualizar datos");
        await Task.Delay(1000);
        await _lamparaService.ConsultarEsp32Async();
        await Task.Delay(1000);

        var currentData = await _lamparaService.ObtenerDatosActualesAsync();
        if (currentData != null)
        {
            System.Diagnostics.Debug.WriteLine($"Datos actualizados después de cambio de modo: {currentData.FechaHora}");

            string currentState = currentData.EstadoLampara switch
            {
                "1" => "Full",
                "2" => "Luz Tenue",
                "3" => "Lámpara Temperatura",
                "4" => "Apagado",
                _ => "Apagado"
            };
            SelectedState = currentState;

            AmbientTemperature = currentData.TemperaturaSensor;
            AmbientHumidity = currentData.HumedadSensor;
            ClimateTemperature = currentData.TemperaturaApi;
            ClimateHumidity = currentData.HumedadApi;
        }

        IsLoading = false;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            System.Diagnostics.Debug.WriteLine("Iniciando LoadDataAsync - Obteniendo datos actuales");

            var currentData = await _lamparaService.ObtenerDatosActualesAsync();

            if (currentData != null)
            {
                System.Diagnostics.Debug.WriteLine($"Datos actuales obtenidos: {currentData.FechaHora}");
                System.Diagnostics.Debug.WriteLine($"Datos: TempSensor={currentData.TemperaturaSensor}, HumSensor={currentData.HumedadSensor}, TempApi={currentData.TemperaturaApi}, HumApi={currentData.HumedadApi}");

                string currentState = currentData.EstadoLampara switch
                {
                    "1" => "Full",
                    "2" => "Luz Tenue",
                    "3" => "Lámpara Temperatura",
                    "4" => "Apagado",
                    _ => "Apagado"
                };
                SelectedState = currentState;

                AmbientTemperature = currentData.TemperaturaSensor;
                AmbientHumidity = currentData.HumedadSensor;
                ClimateTemperature = currentData.TemperaturaApi;
                ClimateHumidity = currentData.HumedadApi;

                System.Diagnostics.Debug.WriteLine($"Datos actualizados: TempAmb={AmbientTemperature}, HumAmb={AmbientHumidity}, TempClima={ClimateTemperature}, HumClima={ClimateHumidity}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No se obtuvieron datos actuales - la API puede no estar accesible");
                await Shell.Current.DisplayAlertAsync("Error", "No se pudieron obtener datos de la API. Verifica que la API esté corriendo y accesible desde la red.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en LoadDataAsync: {ex.Message}");
            await Shell.Current.DisplayAlertAsync("Error de conexión", $"No se pudo conectar a la API: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshDataAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Iniciando RefreshDataAsync - Consultando ESP32");
            var success = await _lamparaService.ConsultarEsp32Async();
            System.Diagnostics.Debug.WriteLine($"ConsultarEsp32Async retornó: {success}");

            await Task.Delay(1000);

            var currentData = await _lamparaService.ObtenerDatosActualesAsync();
            if (currentData != null)
            {
                System.Diagnostics.Debug.WriteLine($"Datos actualizados después de refresh: {currentData.FechaHora}");

                string currentState = currentData.EstadoLampara switch
                {
                    "1" => "Full",
                    "2" => "Luz Tenue",
                    "3" => "Lámpara Temperatura",
                    "4" => "Apagado",
                    _ => "Apagado"
                };
                SelectedState = currentState;

                AmbientTemperature = currentData.TemperaturaSensor;
                AmbientHumidity = currentData.HumedadSensor;
                ClimateTemperature = currentData.TemperaturaApi;
                ClimateHumidity = currentData.HumedadApi;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en RefreshDataAsync: {ex.Message}");
        }
    }

    private async Task LoadCurrentLampStateAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Cargando estado actual de la lámpara...");
            var readings = await _lamparaService.ObtenerTodasLasLecturasAsync();
            System.Diagnostics.Debug.WriteLine($"Lecturas obtenidas: {readings?.Count ?? 0}");

            if (readings != null && readings.Count > 0)
            {
                var latestReading = readings.OrderByDescending(r => r.FechaHora).FirstOrDefault();
                if (latestReading != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Lectura más reciente: {latestReading.FechaHora}, Estado: {latestReading.EstadoLampara}");
                    System.Diagnostics.Debug.WriteLine($"Datos: TempSensor={latestReading.TemperaturaSensor}, HumSensor={latestReading.HumedadSensor}, TempApi={latestReading.TemperaturaApi}, HumApi={latestReading.HumedadApi}");

                    string currentState = latestReading.EstadoLampara switch
                    {
                        "1" => "Full",
                        "2" => "Luz Tenue",
                        "3" => "Lámpara Temperatura",
                        "4" => "Apagado",
                        _ => "Apagado"
                    };
                    SelectedState = currentState;

                    AmbientTemperature = latestReading.TemperaturaSensor;
                    AmbientHumidity = latestReading.HumedadSensor;
                    ClimateTemperature = latestReading.TemperaturaApi;
                    ClimateHumidity = latestReading.HumedadApi;

                    System.Diagnostics.Debug.WriteLine($"Datos actualizados: TempAmb={AmbientTemperature}, HumAmb={AmbientHumidity}, TempClima={ClimateTemperature}, HumClima={ClimateHumidity}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No se encontró lectura más reciente");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No hay lecturas disponibles - usando valores por defecto");
                AmbientTemperature = 25;
                AmbientHumidity = 60;
                ClimateTemperature = 20;
                ClimateHumidity = 70;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando estado actual: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Usando valores por defecto");
            AmbientTemperature = 25;
            AmbientHumidity = 60;
            ClimateTemperature = 20;
            ClimateHumidity = 70;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
