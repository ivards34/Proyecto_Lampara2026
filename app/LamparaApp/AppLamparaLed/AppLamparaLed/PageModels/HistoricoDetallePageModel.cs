using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppLamparaLed.PageModels;

public class HistoricoDetallePageModel : INotifyPropertyChanged
{
    private DateTime _selectedDate = DateTime.Now;
    private List<double> _temperatures = new();
    private List<double> _humidity = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            _selectedDate = value;
            OnPropertyChanged();
        }
    }

    public List<double> Temperatures
    {
        get => _temperatures;
        set
        {
            _temperatures = value;
            OnPropertyChanged();
        }
    }

    public List<double> Humidity
    {
        get => _humidity;
        set
        {
            _humidity = value;
            OnPropertyChanged();
        }
    }

    public HistoricoDetallePageModel()
    {
        GenerateSampleData();
    }

    private void GenerateSampleData()
    {
        var random = new Random();
        _temperatures = new List<double>();
        _humidity = new List<double>();

        for (int i = 0; i < 24; i++)
        {
            _temperatures.Add(20 + random.NextDouble() * 10);
            _humidity.Add(40 + random.NextDouble() * 30);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
