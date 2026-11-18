using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace AppLamparaLed.Controls;

public class DualLineChartView : SKCanvasView
{
    private List<double> _sensorData = new();
    private List<double> _apiData = new();
    private List<string> _labels = new();

    public static readonly BindableProperty SensorDataProperty =
        BindableProperty.Create(
            nameof(SensorData), 
            typeof(List<double>), 
            typeof(DualLineChartView),
            new List<double>(), 
            BindingMode.OneWay,
            propertyChanged: OnDataPropertyChanged);

    public static readonly BindableProperty ApiDataProperty =
        BindableProperty.Create(
            nameof(ApiData), 
            typeof(List<double>), 
            typeof(DualLineChartView),
            new List<double>(),
            BindingMode.OneWay,
            propertyChanged: OnDataPropertyChanged);

    public static readonly BindableProperty LabelsProperty =
        BindableProperty.Create(
            nameof(Labels), 
            typeof(List<string>), 
            typeof(DualLineChartView),
            new List<string>(),
            BindingMode.OneWay,
            propertyChanged: OnDataPropertyChanged);

    public static readonly BindableProperty UnitProperty =
        BindableProperty.Create(
            nameof(Unit),
            typeof(string),
            typeof(DualLineChartView),
            "");

    public List<double> SensorData
    {
        get => (List<double>)GetValue(SensorDataProperty) ?? new();
        set => SetValue(SensorDataProperty, value);
    }

    public List<double> ApiData
    {
        get => (List<double>)GetValue(ApiDataProperty) ?? new();
        set => SetValue(ApiDataProperty, value);
    }

    public List<string> Labels
    {
        get => (List<string>)GetValue(LabelsProperty) ?? new();
        set => SetValue(LabelsProperty, value);
    }

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    private static void OnDataPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DualLineChartView chartView)
        {
            chartView._sensorData = chartView.SensorData ?? new();
            chartView._apiData = chartView.ApiData ?? new();
            chartView._labels = chartView.Labels ?? new();
            chartView.InvalidateSurface();
        }
    }

    public DualLineChartView()
    {
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        try
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            if (_sensorData == null || _sensorData.Count == 0 || 
                _apiData == null || _apiData.Count == 0)
            {
                return;
            }

            int count = Math.Min(_sensorData.Count, _apiData.Count);
            if (count < 2) return;

            SKImageInfo info = e.Info;
            float width = info.Width;
            float height = info.Height;

            // Márgenes para los ejes
            // Espacio suficiente para que las referencias Y queden fuera de la línea vertical.
            float leftPadding = 88;
            float rightPadding = 15;
            float topPadding = 15;
            float bottomPadding = 50;
            float graphWidth = width - leftPadding - rightPadding;
            float graphHeight = height - topPadding - bottomPadding;

            if (graphWidth <= 0 || graphHeight <= 0) return;

            // Escala fija para que los valores de referencia no dependan de las lecturas.
            double minValue = Unit == "°C" ? -5 : 0;
            double maxValue = Unit == "°C" ? 45 : 100;
            double range = maxValue - minValue;

            // ==================== DIBUJAR EJES ====================
            using (var axisPaint = new SKPaint { Color = SKColors.Black, StrokeWidth = 2, IsAntialias = true })
            {
                canvas.DrawLine(leftPadding, topPadding, leftPadding, topPadding + graphHeight, axisPaint);
                canvas.DrawLine(leftPadding, topPadding + graphHeight, leftPadding + graphWidth, topPadding + graphHeight, axisPaint);
            }

            // ==================== ETIQUETAS EJE Y (VALORES CON UNIDAD Y LÍNEAS TICKS) ====================
            int ySteps = 5;
            for (int i = 0; i < ySteps; i++)
            {
                double value = minValue + (i * range) / (ySteps - 1);
                float y = topPadding + graphHeight - (i * graphHeight) / (ySteps - 1);

                // Dibujar línea tick
                using (var tickPaint = new SKPaint { Color = SKColors.Gray, StrokeWidth = 1 })
                {
                    canvas.DrawLine(leftPadding - 5, y, leftPadding, y, tickPaint);
                }

                // DrawAxisText usa x como inicio del texto, no como alineación derecha.
                // Por eso la etiqueta debe comenzar bastante antes del eje vertical.
                DrawAxisText(canvas, value.ToString("F1") + Unit, 4, y + 4, 12);

            }

            // ==================== ETIQUETAS EJE X (TIEMPO Y LÍNEAS TICKS) ====================
            int xSteps = Math.Min(6, count);
            int xStep = Math.Max(1, count / xSteps);

            for (int i = 0; i < count; i += xStep)
            {
                float x = leftPadding + (i * graphWidth) / (count - 1);

                // Dibujar línea tick
                using (var tickPaint = new SKPaint { Color = SKColors.Gray, StrokeWidth = 1 })
                {
                    canvas.DrawLine(x, topPadding + graphHeight, x, topPadding + graphHeight + 5, tickPaint);
                }

                if (i < _labels.Count)
                {
                    DrawAxisText(canvas, _labels[i], x - 12, topPadding + graphHeight + 18, 10);
                }
            }

            // ==================== LÍNEAS DE CUADRÍCULA ====================
            // Grid horizontal (punteado)
            using (var gridPaint = new SKPaint 
            { 
                Color = SKColor.Parse("#E0E0E0"), 
                StrokeWidth = 0.5f, 
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0)
            })
            {
                for (int i = 1; i < ySteps - 1; i++)
                {
                    float y = topPadding + graphHeight - (i * graphHeight) / (ySteps - 1);
                    canvas.DrawLine(leftPadding, y, leftPadding + graphWidth, y, gridPaint);
                }
            }

            // Grid vertical
            using (var gridPaint = new SKPaint 
            { 
                Color = SKColor.Parse("#F0F0F0"), 
                StrokeWidth = 0.3f, 
                IsAntialias = true
            })
            {
                for (int i = 0; i < count; i += xStep)
                {
                    float x = leftPadding + (i * graphWidth) / (count - 1);
                    canvas.DrawLine(x, topPadding, x, topPadding + graphHeight, gridPaint);
                }
            }

            // ==================== LÍNEA SENSOR (ROJO - PUNTEADA) ====================
            using (var sensorPaint = new SKPaint 
            { 
                Color = SKColor.Parse("#FF0000"), 
                StrokeWidth = 3f, 
                IsAntialias = true, 
                Style = SKPaintStyle.Stroke, 
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                PathEffect = SKPathEffect.CreateDash(new float[] { 8, 4 }, 0)
            })
            {
                using var builder = new SKPathBuilder();
                for (int i = 0; i < count; i++)
                {
                    float x = leftPadding + (i * graphWidth) / (count - 1);
                    float y = topPadding + graphHeight - (float)((_sensorData[i] - minValue) / range * graphHeight);

                    if (i == 0)
                        builder.MoveTo(x, y);
                    else
                        builder.LineTo(x, y);
                }
                using SKPath sensorPath = builder.Detach();
                canvas.DrawPath(sensorPath, sensorPaint);
            }

            // ==================== LÍNEA API (AZUL - CONTINUA) ====================
            using (var apiPaint = new SKPaint 
            { 
                Color = SKColor.Parse("#0000FF"), 
                StrokeWidth = 3f, 
                IsAntialias = true, 
                Style = SKPaintStyle.Stroke, 
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            })
            {
                using var builder = new SKPathBuilder();
                for (int i = 0; i < count; i++)
                {
                    float x = leftPadding + (i * graphWidth) / (count - 1);
                    float y = topPadding + graphHeight - (float)((_apiData[i] - minValue) / range * graphHeight);

                    if (i == 0)
                        builder.MoveTo(x, y);
                    else
                        builder.LineTo(x, y);
                }
                using SKPath apiPath = builder.Detach();
                canvas.DrawPath(apiPath, apiPaint);
            }

            // ==================== PUNTOS DE DATOS ====================
            // Puntos Sensor (RED)
            using (var sensorPointPaint = new SKPaint 
            { 
                Color = SKColor.Parse("#FF0000"), 
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            })
            {
                for (int i = 0; i < count; i++)
                {
                    float x = leftPadding + (i * graphWidth) / (count - 1);
                    float y = topPadding + graphHeight - (float)((_sensorData[i] - minValue) / range * graphHeight);
                    canvas.DrawCircle(x, y, 3f, sensorPointPaint);
                }
            }

            // Puntos API (BLUE)
            using (var apiPointPaint = new SKPaint 
            { 
                Color = SKColor.Parse("#0000FF"), 
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            })
            {
                for (int i = 0; i < count; i++)
                {
                    float x = leftPadding + (i * graphWidth) / (count - 1);
                    float y = topPadding + graphHeight - (float)((_apiData[i] - minValue) / range * graphHeight);
                    canvas.DrawCircle(x, y, 3f, apiPointPaint);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en DualLineChartView: {ex.Message}");
        }
    }

    private static void DrawAxisText(SKCanvas canvas, string text, float x, float baseline, float size)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        using var font = new SKFont(SKTypeface.Default, size);
        using var blob = SKTextBlob.Create(text, font);
        canvas.DrawText(blob, x, baseline, paint);
    }

}
