using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages;

public partial class AjustesPage : ContentPage
{
    public AjustesPage(AjustesPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
