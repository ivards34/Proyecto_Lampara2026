using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages;

public partial class HistoricoDetallePage : ContentPage
{
    public HistoricoDetallePage()
    {
        InitializeComponent();
        BindingContext = new HistoricoDetallePageModel();
    }
}
