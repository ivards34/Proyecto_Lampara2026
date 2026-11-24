using Microsoft.Maui.Storage;

namespace AppLamparaLed.Services;

public static class AppSettings
{
    private const string ApiBaseUrlKey = "ApiBaseUrl";

    public const string DefaultApiBaseUrl = "http://192.168.2.102:5000/api/v1/lampara";

    public static string ApiBaseUrl
    {
        get => Preferences.Default.Get(ApiBaseUrlKey, DefaultApiBaseUrl);
        set => Preferences.Default.Set(ApiBaseUrlKey, value);
    }
}
