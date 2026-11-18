using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages;

public partial class MiLamparaPage : ContentPage
{
    private MiLamparaPageModel? _model;

    public MiLamparaPage(MiLamparaPageModel model)
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
