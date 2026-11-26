# App — AppLamparaLed (.NET MAUI)

Aplicación móvil en .NET MAUI para monitorear la lámpara LED: temperatura/humedad del
ambiente, comparación con el clima exterior y control del modo de iluminación.

## Pantallas

Definidas en `AppShell.xaml` (Flyout):

- **Mi Lámpara** (`MiLamparaPage`): estado actual de la lámpara, temperatura y humedad
  del sensor vs. clima de la API, y los 4 modos de luz (Full, Luz Tenue, Lámpara
  Temperatura, Apagado).
- **Histórico** (`HistoricoPage`): selector de fecha + período (Diario / Semanal /
  Mensual) con gráficos de Microcharts/SkiaSharp de temperatura y humedad (sensor y API).

> La solución contiene páginas y modelos del template de MAUI que aún no se usan en el
> `AppShell` (`MainPage`, `ProjectListPage`, `TaskDetailPage`, etc.).

## Requisitos

- .NET 10 SDK con workloads de MAUI:

```bash
dotnet workload install maui
```

- Para Android: Android SDK + JDK 17 (instalados por Visual Studio o `dotnet workload`).

## Ejecutar

Desde Visual Studio: abrir `AppLamparaLed.slnx` y ejecutar en un emulador o dispositivo
Android físico.

Desde línea de comandos:

```bash
cd app/LamparaApp/AppLamparaLed
dotnet build AppLamparaLed/AppLamparaLed.csproj -f net10.0-android
dotnet build -t:Run -f net10.0-android   # despliega en el dispositivo/emulador conectado
```

## Configuración de red

La URL de la API está fija en `Services/LamparaService.cs`:

```csharp
private const string ApiUrl = "http://192.168.1.9:5000/api/v1/lampara";
```

- `192.168.1.9` → IP de la PC donde corre `LamparaLedAPI`.
- `5000` → puerto de la API.

**Ajustar la IP** según la red local (la PC y el celular deben estar en la misma WiFi).
La API debe escuchar en `0.0.0.0` (ya configurado) y el celular necesita permiso de
internet (declarado en `AndroidManifest.xml`).

## Estructura relevante

```
AppLamparaLed/
├── Pages/            # XAML + code-behind de las pantallas
├── PageModels/       # Lógica (INotifyPropertyChanged manual)
├── Services/         # ILamparaService / LamparaService (HttpClient → API)
├── Models/           # LecturaHistorica, etc.
└── Controls/         # DualLineChartView y controles propios
```

Las llamadas a la API están en `Services/LamparaService.cs`:
`GET .../actual`, `GET .../lecturas`, `GET .../consultar`, `POST .../modo`.
Ver el [contrato de la API](../docs/contrato-api.md).
