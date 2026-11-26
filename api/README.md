# API — LamparaLedAPI

Web API en ASP.NET Core (.NET 10) con arquitectura en capas
(`Presentacion → Negocio → Infraestructura`). Recibe/persiste las lecturas del ESP32,
consulta el clima exterior (WeatherAPI) y sirve los datos a la app móvil.

## Estructura

```
LamparaLedAPI/
├── LamparaLedAPI.slnx
├── Presentacion/      # Controllers (LamparaController), Program.cs (DI, pipeline),
│                      # ESP32BackgroundService (sondeo al ESP32)
├── Negocio/           # LamparaService (orquestación), ClimaService (IClimaService),
│                      # DTOs (DatosLamparaDTO, ClimaDTO)
└── Infraestructura/   # LamparaDbContext (EF Core + Npgsql), LamparaRepository,
                       # ESP32Client (IESP32Client), ConectorWeatherAPI
                       # (IConnectorMeteorologiaAPI), migraciones
```

## Requisitos

- .NET 10 SDK
- PostgreSQL corriendo con la base `lamparadb` creada
- (Opcional) `dotnet-ef` para migraciones: `dotnet tool install --global dotnet-ef`

## Configuración

`Presentacion/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=lamparadb;Username=[usuario];Password=[password]"
  },
  "WeatherAPI": {
    "ApiKey": "[api-key]",
    "BaseUrl": "http://api.weatherapi.com/",
    "Ubicacion": "Eduardo Castex, Argentina"
  },
  "ESP32": {
    "BaseUrl": "http://192.168.1.102",
    "IntervaloMinutos": 30
  }
}
```

Ajustar según la red local:

- `ESP32:BaseUrl` → IP que imprimió el ESP32 al arrancar.
- `ESP32:IntervaloMinutos` → cada cuánto el `ESP32BackgroundService` consulta al ESP32.
- `ConnectionStrings` → credenciales de PostgreSQL.
- `WeatherAPI:ApiKey` / `BaseUrl` / `Ubicacion` → credenciales, URL base y ubicación del servicio meteorológico.

> En producción usar variables de entorno o User Secrets en lugar de valores reales en
> `appsettings.json`.

## Ejecutar

```bash
cd api/LamparaLedAPI
dotnet restore
dotnet run --project Presentacion
```

La API escucha en **`http://0.0.0.0:5000`** (todas las interfaces) para que la app y el
ESP32 la alcancen desde otros dispositivos de la red.

### Base de datos

Las migraciones están en `Infraestructura/Migrations`. Para aplicarlas:

```bash
cd api/LamparaLedAPI
dotnet ef database update --project Infraestructura --startup-project Presentacion
```

## Endpoints

Ver el [contrato completo de la API](../docs/contrato-api.md). Resumen:

| Método |             Ruta            |                 Qué hace                  |
|--------|-----------------------------|-------------------------------------------|
|  GET   | `/api/v1/Lampara/actual`    | Última lectura                            |
|  GET   | `/api/v1/Lampara/lecturas`  | Todas las lecturas                        |
|  POST  | `/api/v1/Lampara`           | Recibe JSON del ESP32                     |
|  POST  | `/api/v1/Lampara/modo`      | Cambia modo de luz (`{ "modoId": 1..4 }`) |
|  GET   | `/api/v1/Lampara/consultar` | Fuerza consulta al ESP32                  |

## Cómo fluye el dato

`ESP32BackgroundService` consulta al ESP32 cada `IntervaloMinutos` → `LamparaService` obtiene clima vía `IClimaService` → `IConnectorMeteorologiaAPI` → `ConectorWeatherAPI` (WeatherAPI) → `LamparaRepository` guarda en PostgreSQL → la app consume los endpoints `GET`.
