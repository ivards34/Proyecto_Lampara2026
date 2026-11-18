using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Microcharts.Maui;
using AppLamparaLed.Services;
using AppLamparaLed.PageModels;
using AppLamparaLed.Pages;

namespace AppLamparaLed
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .UseMicrocharts()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddHttpClient<ILamparaService, LamparaService>();
            builder.Services.AddTransient<MiLamparaPageModel>();
            builder.Services.AddTransient<HistoricoPageModel>();
            builder.Services.AddTransient<AjustesPageModel>();
            builder.Services.AddTransient<MiLamparaPage>();
            builder.Services.AddTransient<HistoricoPage>();
            builder.Services.AddTransient<AjustesPage>();

            return builder.Build();
        }
    }
}
