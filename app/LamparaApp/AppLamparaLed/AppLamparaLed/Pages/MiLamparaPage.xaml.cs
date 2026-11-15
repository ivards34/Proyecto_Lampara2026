using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages;

public partial class MiLamparaPage : ContentPage
{
    private MiLamparaPageModel? _model;

    public MiLamparaPage()
    {
        InitializeComponent();
        _model = new MiLamparaPageModel();
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
