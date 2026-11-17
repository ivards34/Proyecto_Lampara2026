# Firmware — LamparaLed_ESP32

Sketch de Arduino para el ESP32-C3 mini que controla la lámpara LED y expone sus datos
por HTTP.

## Hardware

| Componente    | Detalle               | Pin           |
|---------------|-----------------------|---------------|
| ESP32-C3 mini | Controlador           |       —       |
| DHT11         | Temperatura y humedad | GPIO 10       |
| Tira WS2812B  | 128 LEDs RGB          | GPIO 4 (DATA) |
| Sensor táctil | Cambio de modo        | GPIO 7        |

La hora se sincroniza por NTP (`pool.ntp.org`, UTC-3 Argentina).

## Librerías necesarias (Arduino IDE → Library Manager)

- **FastLED** (control de la tira WS2812B)
- **DHT sensor library** (Adafruit)
- **Bounce2** (anti-rebote del sensor táctil)
- **WiFi / WebServer / time** (core ESP32, ya incluidas)

## Cómo cargar el código

1. Abrir `LamparaLed_ESP32.ino` en Arduino IDE 1.8.19.
2. En **Archivo → Preferencias**, agregar la URL del core ESP32
   (`https://espressif.github.io/arduino-esp32/package_esp32_index.json`) e instalarlo.
3. En **Herramientas → Placa**, seleccionar **ESP32C3 Dev Module**.
4. Editar las credenciales WiFi al inicio del archivo:

```cpp
const char* ssid     = "TU_SSID";
const char* password = "TU_PASSWORD";
```

5. Conectar el ESP32 por USB, seleccionar el puerto y subir con el botón →.

Al arrancar, el Monitor Serie (115200 baudios) muestra la IP asignada por el DHCP.
**Anotar esa IP**: hay que configurarla en la API (`ESP32:BaseUrl`) y en la app
(`ApiUrl`). Lo ideal es reservar la IP en el router o usar IP estática.

## Endpoints que expone el ESP32

| Ruta                        | Método |                           Qué hace                               |
|-----------------------------|--------|------------------------------------------------------------------|
| `/api/v1/lampara`           |   GET  | Devuelve JSON con fecha/hora, temperatura, humedad y modo actual |
| `/api/v1/lampara/modo?id=N` |   GET  | Cambia el modo de luz (`id`: 1-4)                                |

## Modos de luz

| ID |                      Modo                           |
|----|-----------------------------------------------------|
|  1 | Blanco completo                                     |
|  2 | Blanco tenue                                        |
|  3 | Color según temperatura (azul→verde→rojo, 10-40 °C) |
|  4 | Apagado                                             |

El modo también se cambia tocando el sensor táctil: cada toque cicla 1 → 2 → 3 → 4.
