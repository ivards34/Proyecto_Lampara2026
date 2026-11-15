using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages;

public partial class HistoricoPage : ContentPage
{
    private HistoricoPageModel? _model;

    public HistoricoPage()
    {
        InitializeComponent();
        _model = new HistoricoPageModel();
        BindingContext = _model;
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
