#include <WiFi.h>
#include <WebServer.h> // <- Librería para que el ESP32 sea el Servidor

#include <Bounce2.h>
#include "DHT.h"

#include <FastLED.h>
#include "time.h"

// --- CONFIGURACIÓN DE PINES Y HARDWARE ---
#define FASTLED_ALLOW_INTERRUPTS 0

#define DHTTYPE DHT11 
#define NUM_LEDS 128 
#define DATA_PIN 4        // Pin tira de LEDs
#define TOUCH_PIN 7       // Pin del Sensor Táctil
#define DHTPIN 10         // Pin del Sensor DHT11

#define BRIGHTNESS 50 
#define LED_TYPE WS2812B 
#define COLOR_ORDER GRB 

void conectarWiFi();

// --- OBJETOS DE HARDWARE Y LIBRERÍAS ---
CRGB leds[NUM_LEDS]; 
DHT dht(DHTPIN, DHTTYPE); 
Bounce touch = Bounce(); 

// Instancia del servidor HTTP en el puerto estándar 80
WebServer server(80); 

const char* ntpServer = "pool.ntp.org";
const long  gmtOffset_sec = -10800;   // UTC-3 Argentina
const int   daylightOffset_sec = 0;

// --- VARIABLES GLOBALES ---
short int funcionActual = 4;        // 1: Blanco, 2: Blanco tenue, 3: Color temp, 4: Apagado
float temperaturaActual = 20.0;     // Valor por defecto/inicial
float humedadActual = 50.0;

unsigned long previousMillisAPI = 0;
const long intervalAPI = 5000;     // Intervalo para leer DHT (5 seg)

// --- PROTOTIPOS DE FUNCIONES ---
void imprimirHoraActual();
void colorBlanco();
void colorBlancoTenue();
void apagarLED();
void colorPorTemperatura(float temperatura);
String obtenerFechaHora();
const char* obtenerEstadoLuz();

// CORREGIDO: Se quitaron por completo los parámetros del prototipo
void atenderPeticionAPI(); 
void cambiarModoAPI();     // NUEVA FUNCIÓN para recibir órdenes de la API

void aplicarModo(int modo);

// Sustituir con datos de tu red
const char* ssid     = "Sosa";
const char* password = "bartolo2";

void setup() {
  delay(2000); // Tiempo de seguridad para ESP32-C3
  Serial.begin(115200); 
  Serial.println("Iniciando");

  conectarWiFi();
  
  dht.begin(); 
  
  // Configuración del sensor táctil con Bounce2 e INPUT_PULLDOWN
  touch.attach(TOUCH_PIN, INPUT_PULLDOWN);
  touch.interval(15);
  
  // Configurar reloj con NTP
  configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);
  
  // Inicialización de LEDs
  FastLED.addLeds<LED_TYPE, DATA_PIN, COLOR_ORDER>(leds, NUM_LEDS);
  FastLED.setBrightness(BRIGHTNESS); 

  // --- CONFIGURACIÓN DE ENRUTAMIENTO DEL SERVIDOR ---
  // Obtener datos de la lámpara
  server.on("/api/v1/lampara", HTTP_GET, atenderPeticionAPI);
  
  // NUEVO ENDPOINT: Cambiar el estado/modo de la lámpara
  server.on("/api/v1/lampara/modo", HTTP_GET, cambiarModoAPI);
  
  server.begin(); // Arrancar el servidor HTTP
  Serial.println("Servidor HTTP listo para recibir peticiones de la API."); 

  Serial.println("loop"); 
}

void loop() 
{
  // Escucha constantemente si la API externa está haciendo una petición
  server.handleClient();

  unsigned long tiempoActual = millis();
  
  // ACTUALIZAR ESTADO DEL SENSOR TÁCTIL
  touch.update();

  // CAMBIO DE MODO AL TOCAR EL SENSOR (touch.changed())
  if (touch.rose()) 
  { 
    funcionActual++;
    if (funcionActual > 4) 
      funcionActual = 1;
    
    Serial.print(F(">>> Botón táctil detectado! Modo cambiado a: "));
    Serial.println(funcionActual);

    // Aplicar la función de iluminación directamente
    aplicarModo(funcionActual);
  }

  // LECTURA DE SENSOR DHT11 (Cada 15 segundos)
  if (tiempoActual - previousMillisAPI >= intervalAPI)
  {
    previousMillisAPI = tiempoActual;

    // Leemos directamente sobre variables temporales para validar
    float t = dht.readTemperature();
    float h = dht.readHumidity();

    if (isnan(h) || isnan(t))
    {
      Serial.println(F("Error DHT11!"));
    }
    else
    {
      temperaturaActual = t;
      humedadActual = h;
      imprimirHoraActual();
    }

    // Si el modo activo es temperatura, actualizamos el color en vivo
    if (funcionActual == 3)
      colorPorTemperatura(temperaturaActual);
  }
}

// ==============================================================
// ATENCIÓN DE PETICIONES HTTP (RESPUESTA A TU API)
// ==============================================================
// CORREGIDO: Firma vacía para cumplir los requisitos de WebServer.h
void atenderPeticionAPI() 
{
  char jsonBuffer[250]; 

  // Estructuramos el JSON usando directamente las variables globales
  snprintf(jsonBuffer, sizeof(jsonBuffer),
           "{\"fecha_hora\":\"%s\",\"dispositivo\":\"ESP32_Lampara\",\"temperatura\":%.2f,\"humedad\":%.2f,\"lampara\":\"%s\"}",
           obtenerFechaHora().c_str(), temperaturaActual, humedadActual, obtenerEstadoLuz());
  
  server.send(200, "application/json", jsonBuffer);
  Serial.println(F("¡Datos de estado enviados con éxito a la API externa!"));
}

// ==============================================================
// NUEVA FUNCIÓN: RECIBIR ORDEN DE CAMBIO DE MODO DESDE LA API
// ==============================================================
void cambiarModoAPI() 
{
  // Verifica si la petición incluye el parámetro "?id="
  if (server.hasArg("id")) 
  {
    String idParam = server.arg("id");
    int nuevoModo = idParam.toInt();

    // Validamos que el modo esté dentro del rango correcto (1 al 4)
    if (nuevoModo >= 1 && nuevoModo <= 4) 
    {
      funcionActual = nuevoModo;
      aplicarModo(funcionActual); // Aplica los colores de inmediato
      
      Serial.print(F(">>> Modo cambiado vía API a: "));
      Serial.println(funcionActual);

      // Respondemos un JSON confirmando el éxito del cambio
      server.send(200, "application/json", "{\"status\":\"ok\",\"mensaje\":\"Modo actualizado\"}");
    } 
    else 
    {
      server.send(400, "application/json", "{\"status\":\"error\",\"mensaje\":\"Modo invalido (Debe ser 1-4)\"}");
    }
  } 
  else 
  {
    server.send(400, "application/json", "{\"status\":\"error\",\"mensaje\":\"Falta parametro 'id'\"}");
  }
}

// ==============================================================
// FUNCIONES DE CONTROL DE ILUMINACIÓN
// ==============================================================
void colorPorTemperatura(float temp) {
  temp = constrain(temp, 10.0, 40.0);

  if (temp <= 20.0) {
    uint8_t verde = map(temp, 10, 20, 0, 255);
    uint8_t azul  = map(temp, 10, 20, 255, 0);
    fill_solid(leds, NUM_LEDS, CRGB(0, verde, azul));
  } else {
    uint8_t rojo  = map(temp, 20, 40, 0, 255);
    uint8_t verde = map(temp, 20, 40, 255, 0);
    fill_solid(leds, NUM_LEDS, CRGB(rojo, verde, 0));
  }
  FastLED.show();
}

void colorBlanco() {
  fill_solid(leds, NUM_LEDS, CRGB::White);
  FastLED.show();
}

void colorBlancoTenue() {
  fill_solid(leds, NUM_LEDS, CRGB(60, 60, 60));
  FastLED.show();
}

void apagarLED() {
  fill_solid(leds, NUM_LEDS, CRGB::Black);
  FastLED.show();
}

// ==============================================================
// FUNCIONES NTP Y RELOJ
// ==============================================================
void imprimirHoraActual() {
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo)) {
    return;
  }
}

String obtenerFechaHora() {
  struct tm timeinfo;
  if (getLocalTime(&timeinfo)) {
    char buffer[25]; 
    strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S", &timeinfo);
    return String(buffer);
  } else {
    return "0000-00-00 00:00:00";
  }
}

const char* obtenerEstadoLuz() {
  switch (funcionActual) {
    case 1: return "1";
    case 2: return "2";
    case 3: return "3";
    case 4: return "4";
    default: return "0";
  }
}

void conectarWiFi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  WiFi.setTxPower(WIFI_POWER_8_5dBm); 

  Serial.print("Conectando a:\t");
  Serial.println(ssid);

  while (WiFi.status() != WL_CONNECTED) {
    delay(2000);
    Serial.println('.');
  }

  Serial.println();
  Serial.print("Conectado con éxito. IP del ESP32:\t");
  Serial.println(WiFi.localIP()); 
}

void aplicarModo(int modo) {
  switch (modo) {
    case 1: colorBlanco(); break;
    case 2: colorBlancoTenue(); break;
    case 3: colorPorTemperatura(temperaturaActual); break;
    case 4: apagarLED(); break;
  }
}
