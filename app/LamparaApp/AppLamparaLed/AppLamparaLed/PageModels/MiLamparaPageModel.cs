using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AppLamparaLed.Services;
using AppLamparaLed.Models;

namespace AppLamparaLed.PageModels;

public class MiLamparaPageModel : INotifyPropertyChanged
{
    private readonly ILamparaService _lamparaService;
    private ModoLampara _selectedModo = ModoLampara.Apagado;
    private double _ambientTemperature = 0;
    private double _ambientHumidity = 0;
    private double _climateTemperature = 0;
    private double _climateHumidity = 0;
    private bool _isLoading = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MiLamparaPageModel(ILamparaService lamparaService)
    {
        _lamparaService = lamparaService;
    }

    public ModoLampara SelectedModo
    {
        get => _selectedModo;
        set
        {
            _selectedModo = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsFullSelected));
            OnPropertyChanged(nameof(IsLuzTenueSelected));
            OnPropertyChanged(nameof(IsApagadoSelected));
            OnPropertyChanged(nameof(IsLamparaTemperaturaSelected));
        }
    }

    public bool IsFullSelected => SelectedModo == ModoLampara.Full;
    public bool IsLuzTenueSelected => SelectedModo == ModoLampara.LuzTenue;
    public bool IsApagadoSelected => SelectedModo == ModoLampara.Apagado;
    public bool IsLamparaTemperaturaSelected => SelectedModo == ModoLampara.LamparaTemperatura;

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
        SelectedModo = state.DesdeTexto();

        var success = await _lamparaService.CambiarModoAsync((int)SelectedModo);

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
            AplicarLectura(currentData);
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
                AplicarLectura(currentData);
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
                AplicarLectura(currentData);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en RefreshDataAsync: {ex.Message}");
        }
    }

    private void AplicarLectura(LecturaHistorica lectura)
    {
        SelectedModo = lectura.EstadoLampara.DesdeEstado();
        AmbientTemperature = lectura.TemperaturaSensor;
        AmbientHumidity = lectura.HumedadSensor;
        ClimateTemperature = lectura.TemperaturaApi;
        ClimateHumidity = lectura.HumedadApi;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
