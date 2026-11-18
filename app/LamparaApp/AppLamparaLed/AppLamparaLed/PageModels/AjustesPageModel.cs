using System.Windows.Input;
using AppLamparaLed.Services;

namespace AppLamparaLed.PageModels;

public class AjustesPageModel
{
    public string ApiUrl { get; set; } = AppSettings.ApiBaseUrl;

    public ICommand GuardarCommand => new Command(async () =>
    {
        var url = ApiUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(url))
        {
            await Shell.Current.DisplayAlertAsync("Ajustes", "La URL no puede estar vacía.", "OK");
            return;
        }

        AppSettings.ApiBaseUrl = url;
        await Shell.Current.DisplayAlertAsync("Ajustes", "Configuración guardada. Se aplicará en la próxima consulta a la API.", "OK");
    });
}
