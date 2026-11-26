# Contrato de la API — LamparaLedAPI

Documentación de los endpoints REST de la API. Base URL en desarrollo:
`http://<IP-de-la-PC>:5000` (la API escucha en `0.0.0.0:5000`, ver `Program.cs`).

> En modo desarrollo también está disponible la UI de OpenAPI/Swagger (`app.MapOpenApi`).

## Resumen de endpoints

| Método |            Ruta             |                              Descripción                              |
|--------|-----------------------------|-----------------------------------------------------------------------|
| GET    | `/api/v1/Lampara/consultar` | La API consulta al ESP32 y guarda la lectura              |     —     |
| POST   | `/api/v1/Lampara`           | Recibe datos del sensor (push desde ESP32 u otro cliente) | Body JSON |
| POST   | `/api/v1/Lampara/modo`      | Cambia el modo de la lámpara (se reenvía al ESP32)        | Body JSON |
| GET    | `/api/v1/Lampara/lecturas`  | Devuelve todas las lecturas guardadas                     |     —     |
| GET    | `/api/v1/Lampara/actual`    | Devuelve la última lectura                                |     —     |
| GET    | `/WeatherForecast`          | Endpoint de ejemplo del template (sin uso)                |     —     |

## POST `/api/v1/Lampara`

Recibe el JSON que publica el ESP32. La API lo deserializa, lo enriquece con el clima de
WeatherAPI y lo guarda en `lampara_lecturas`.

**Body** (formato que envía el ESP32 en `GET /api/v1/lampara`):

```json
{
  "fecha_hora": "2026-11-15 14:30:00",
  "dispositivo": "ESP32_Lampara",
  "temperatura": 24.5,
  "humedad": 58.0,
  "lampara": "3"
}
```

| Campo         | Tipo   | Descripción                                        |
|---------------|--------|----------------------------------------------------|
| `fecha_hora`  | string | Fecha/hora local del ESP32 (`yyyy-MM-dd HH:mm:ss`) |
| `dispositivo` | string | Identificador del dispositivo                      |
| `temperatura` | number | Temperatura del sensor DHT11 (°C)                  |
| `humedad`     | number | Humedad relativa del sensor DHT11 (%)              |
| `lampara`     | string | Estado/modo actual: `"1"`-`"4"`                    |

**Respuesta**: `200 OK` — `"Datos guardados correctamente"`

## POST `/api/v1/Lampara/modo`

Cambia el modo de iluminación. La API llama a
`GET http://<ESP32>/api/v1/lampara/modo?id=<modoId>`.

**Body**:

```json
{ "modoId": 3 }
```

**Modos válidos**: `1` = Full, `2` = Luz Tenue, `3` = Lámpara Temperatura, `4` = Apagado.

**Respuesta**: `200 OK` — `"Modo de lámpara cambiado a 3"`

## GET `/api/v1/Lampara/consultar`

Fuerza una consulta inmediata al ESP32 (la misma que hace el `ESP32BackgroundService`
periódicamente). Útil para pruebas manuales.

**Respuesta**: `200 OK` — `"Consulta al ESP32 realizada y datos guardados"`

## GET `/api/v1/Lampara/actual`

Devuelve la lectura más reciente.

**Respuesta** `200 OK`:

```json
{
  "id": 128,
  "fechaHora": "2026-11-15T14:30:00",
  "dispositivoId": "ESP32_Lampara",
  "temperaturaSensor": 24.5,
  "humedadSensor": 58.0,
  "temperaturaApi": 22.0,
  "humedadApi": 60.0,
  "estadoLampara": "3"
}
```

`404 Not Found` si todavía no hay lecturas.

## GET `/api/v1/Lampara/lecturas`

Devuelve **todas** las lecturas guardadas (array, orden según la base). Mismo formato de
objeto que `/actual`. La app móvil lo usa para el histórico.

```json
[
  {
    "id": 128,
    "fechaHora": "2026-11-15T14:30:00",
    "dispositivoId": "ESP32_Lampara",
    "temperaturaSensor": 24.5,
    "humedadSensor": 58.0,
    "temperaturaApi": 22.0,
    "humedadApi": 60.0,
    "estadoLampara": "3"
  }
]
```

## Endpoints del ESP32 (no son de la API, referencia)

El firmware expone su propio servidor HTTP en el puerto 80:

| Método |            Ruta             |                       Descripción                              |
|--------|-----------------------------|----------------------------------------------------------------|
| GET    | `/api/v1/lampara`           | Devuelve el JSON de estado actual (el mismo del body del POST) |
| GET    | `/api/v1/lampara/modo?id=N` | Cambia el modo de luz (`id` entre 1 y 4)                       |

## Notas

- No hay autenticación: los endpoints son públicos dentro de la red local.
- Los nombres de propiedades de respuesta usan camelCase (serialización por defecto de
  ASP.NET Core).
- Errores: los controllers devuelven `400 Bad Request` con el mensaje de excepción.
