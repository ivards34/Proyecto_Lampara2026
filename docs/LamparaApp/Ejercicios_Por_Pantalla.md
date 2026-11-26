# Ejercicios por Pantalla y Función — Ivar (Proyecto Lámpara LED)

**Proyecto**: lámpara LED con sensor de temperatura/humedad (ESP32)  
**Objetivo**: practicar cada concepto en ejercicios pequeños y aislados, **antes** de desarrollar la app real.

> Cada ejercicio va en `ejercicios/NN-nombre/` con su propio README. Uno por concepto.
>
> **Base común**: la app **consulta datos a una API** (temperatura/humedad/estado) y los **transforma en la vista** (números, color, gráfico). Envía el estado por POST. Leer primero la **`Guia_API_y_Visualizacion.md`** (carpeta Proyectos).

**Formato de cada ejercicio**: *Qué hacés* · *Qué investigás* · *Datos (cómo se conecta con la API y cómo se muestra)*.

> **Ajustes**: 4 estados (Full, Regulación %, Apagado, Automático). Color según temperatura. Indicador de conexión (heartbeat). Comparación interna vs externa. Histórico por día con gráfico de dos líneas y dos escalas.

---

## Pantalla: Principal — Datos en tiempo real (GET)

### Ejercicio 1 — Mostrar temperatura, humedad y estado
- **Qué hacés**: pantalla con temperatura, humedad y estado actual en tarjetas.
- **Qué investigás**: `Grid`, `Label`, tarjetas con `Border`.
- **Datos**: por ahora valores a mano; luego vendrán por GET (Ej. 2).

### Ejercicio 2 — GET de datos a la API
- **Qué hacés**: traer temperatura y humedad de la API y mostrarlas.
- **Qué investigás**: `HttpClient.GetAsync`, deserializar JSON, `async/await`.
- **Datos (API → vista)**: GET → clase `Medicion {Temp, Humedad}` → deserializar → `Label`.

### Ejercicio 3 — Estados de carga y error
- **Qué hacés**: `ActivityIndicator` mientras carga; mensaje si no hay conexión.
- **Qué investigás**: `ActivityIndicator`, `try/catch` con red.
- **Datos (API → vista)**: envolver el GET del Ej. 2 y controlar los 3 estados.

### Ejercicio 4 — Comparación interna (lámpara) vs externa (clima)
- **Qué hacés**: mostrar temperatura interna (lámpara) y externa (clima) lado a lado para compararlas.
- **Qué investigás**: dos llamadas GET (o dos campos del JSON), cómo presentarlas para comparar (estilo Weather App).
- **Datos (API → vista)**: dos fuentes → dos valores en pantalla, uno al lado del otro.

---

## Pantalla: Principal — Estados de la lámpara (4 estados)

### Ejercicio 5 — Selector de estado con 4 opciones
- **Qué hacés**: Full, Regulación %, Apagado, Automático; al tocar una se resalta. "Automático" se muestra distinto (lo gestiona la lámpara).
- **Qué investigás**: marcar el activo, distinguir el estado que no controla el teléfono.
- **Datos**: prepara el estado que se enviará por POST.

### Ejercicio 6 — Regulación de intensidad con slider (%)
- **Qué hacés**: un `Slider` para definir la intensidad en % y mostrar el valor.
- **Qué investigás**: `Slider`, `Value`, límites del rango.
- **Datos**: el % elegido acompaña al estado en el POST.

### Ejercicio 7 — Enviar el estado con POST
- **Qué hacés**: al elegir estado o cambiar la intensidad, enviar a la API y mostrar la respuesta.
- **Qué investigás**: serializar a JSON, `HttpClient.PostAsync`, códigos de respuesta.
- **Datos (vista → API)**: objeto `{estado, intensidad}` → JSON → POST.

---

## Función: Mapa de temperatura (color de la lámpara)

### Ejercicio 8 — Colorear según temperatura (celeste → rojo)
- **Qué hacés**: un círculo (la "lámpara") que cambie de color: celeste con frío, rojo con calor.
- **Qué investigás**: interpolación de color o rangos con `if`, `Color.FromRgb`, `Color.Lerp`.
- **Datos (API → vista)**: la temperatura que vino por GET se mapea a un color.

---

## Función: Indicador de conexión (heartbeat)

### Ejercicio 9 — Mostrar si la lámpara está conectada
- **Qué hacés**: indicador (punto verde/rojo + texto). Si no llega un "latido" cada X s, pasa a desconectada.
- **Qué investigás**: `IDispatcherTimer` para el heartbeat, cambiar color/texto según el último latido.
- **Datos (API → vista)**: cada respuesta exitosa del GET actúa como "latido"; si no llega, se marca desconectada.

---

## Pantalla: Histórico (por día, calendario + gráfico)

### Ejercicio 10 — Selector de fecha (calendario)
- **Qué hacés**: un `DatePicker` o calendario arriba; al elegir una fecha, mostrarla.
- **Qué investigás**: `DatePicker`, evento de cambio de fecha.
- **Datos**: la fecha elegida define qué datos se piden por GET.

### Ejercicio 11 — Gráfico de dos líneas con dos escalas (datos → gráfico)
- **Qué hacés**: gráfico con dos líneas (temperatura y humedad), escala izquierda y derecha (estilo Excel), eje X en horas.
- **Qué investigás**: Microcharts `LineChart` (o `GraphicsView`), cómo manejar dos escalas, etiquetar ejes.
- **Datos (API → gráfico)**: GET del día → dos listas (temp por hora, humedad por hora) → dos series de puntos → dos líneas.

### Ejercicio 12 — Ver en horizontal (landscape)
- **Qué hacés**: que el gráfico se vea completo al girar el teléfono.
- **Qué investigás**: orientación de pantalla en MAUI, layout responsive, scroll (calendario arriba, gráfico abajo).
- **Datos**: no cambia los datos, solo la presentación.

---

## Función: base de datos del histórico

### Ejercicio 13 — Tabla simple (time_day, temperatura, humedad)
- **Qué hacés**: guardar mediciones con `time_day` (clave), temperatura y humedad; consultar las de un día.
- **Qué investigás**: `sqlite-net-pcl`, crear tabla, insertar, consultar filtrando por fecha.
- **Datos**: respaldo local; también sirve para dibujar el gráfico sin conexión. (La lámpara guarda automáticamente.)

---

## Función: Navegación

### Ejercicio 14 — Botón para pasar entre Principal e Histórico
- **Qué hacés**: un botón que navegue de la principal al histórico y viceversa.
- **Qué investigás**: `Shell`/`GoToAsync`, rutas.
- **Datos**: no usa API.

---

## Resumen de temas a investigar

| Tema | Dónde se usa | Prioridad |
|------|--------------|-----------|
| **Flujo API → datos → vista/gráfico** (guía común) | Toda la app | Alta |
| `HttpClient` GET + `System.Text.Json` | Tiempo real, Histórico | Alta |
| `HttpClient` POST (estado/intensidad) | Estados | Alta |
| Selección de estado (4 opciones) | Estados | Alta |
| `Slider` de intensidad (%) | Regulación | Alta |
| Comparación interna vs externa | Tiempo real | Alta |
| Mapear temperatura a color | Mapa de temperatura | Alta |
| Carga y errores (`ActivityIndicator`, try/catch) | Tiempo real | Alta |
| Heartbeat / indicador de conexión | Conexión | Media |
| Gráfico de dos líneas + dos escalas (datos → gráfico) | Histórico | Alta |
| `DatePicker` / calendario | Histórico | Alta |
| Orientación horizontal (landscape) | Histórico | Media |
| SQLite (time_day, temp, humedad) | Histórico | Alta |
| Navegación entre pantallas | Toda la app | Media |

---

*Programación III — Aplicaciones Móviles con .NET MAUI — Ciclo Lectivo 2026*
