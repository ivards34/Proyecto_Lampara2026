using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages;

public partial class HistoricoPage : ContentPage
{
    private HistoricoPageModel? _model;

    public HistoricoPage(HistoricoPageModel model)
    {
        InitializeComponent();
        _model = model;
        BindingContext = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_model != null)
        {
            await _model.LoadDataAsync();
        }
    }
}
