using AppLamparaLed.Models;
using AppLamparaLed.PageModels;

namespace AppLamparaLed.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}