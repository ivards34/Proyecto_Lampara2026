# Actividades de Diseño de la App — Ivar (Proyecto Lámpara LED)

**Proyecto**: lámpara LED con sensor de temperatura/humedad (ESP32)  
**Foco de trabajo con el profesor**: diseño de la app (pantallas, layout, colores, navegación).

> Este documento se centra **solo en la app MAUI**. El ESP32, el sensor, el panel LED y la publicación de datos **no los desarrolla el alumno a nivel materia de apps**: la app solo **consume** la API (lee temperatura/humedad) y **envía** el estado de luz elegido.

> **Ajustes acordados**: **4 estados** de lámpara (Full, Regulación %, Apagado, Automático); la lámpara se **colorea según temperatura**; **indicador de conexión** (heartbeat); tiempo real con **comparación interna vs externa**; histórico por **día** con calendario + gráfico horizontal de dos líneas y dos escalas.

---

## 1. Qué trabajamos con vos (alcance app)

- Diseñar el **prototipo de las pantallas**: qué se ve, dónde va cada elemento, colores.
- Diseñar cómo se muestran **temperatura, humedad y estado** en tiempo real.
- Diseñar el **control de estados** (4) y la **regulación de intensidad**.
- Diseñar el **histórico** por día con su gráfico.

### Qué NO entra en tu trabajo (contexto externo)
- El microcontrolador ESP32 y su firmware.
- El sensor de temperatura/humedad y el panel LED.
- Cómo el ESP32 publica los datos y gestiona el estado Automático.

> Para vos: la API es una "caja" que te da temperatura/humedad y a la que le mandás el estado/intensidad de luz.

---

## 2. Entregable de esta etapa

Un **mockup de cada pantalla** con: distribución de elementos, paleta y tipografía, qué dato viene por GET y qué se envía por POST, y cómo se navega.

---

## 3. Pantallas a prototipar

### Pantalla 1 — Principal (tiempo real + estados)
- **Objetivo**: ver el ambiente, comparar interno/externo y controlar la luz.
- **Elementos y ubicación**:
  - **Indicador de conexión** (arriba): "Conectada / Desconectada" (posible heartbeat).
  - **Lámpara coloreada según temperatura** (celeste = frío → rojo = calor).
  - **Temperatura y humedad** ambiente en tarjetas.
  - **Comparación interna (lámpara) vs externa (clima)** — estilo Weather App.
  - **4 estados de luz**: Full, Regulación %, Apagado, Automático. Los estados 1-3 se controlan desde el teléfono; el 4º lo gestiona la lámpara sola (se muestra distinto).
  - **Slider de intensidad (%)** para el estado de regulación.
  - Botón para ir al Histórico.

### Pantalla 2 — Histórico (por día)
- **Objetivo**: ver la evolución de temperatura y humedad de un día.
- **Elementos y ubicación**:
  - **Calendario / selector de fecha** arriba.
  - **Gráfico** abajo (scrolleable): dos líneas — temperatura y humedad — con **dos escalas** (izquierda/derecha, estilo Excel) y el **eje X = hora del día**.
  - Sugerencia: **girar el teléfono a horizontal** para ver el gráfico completo.

---

## 4. Elementos de diseño destacados

### Los 4 estados de la lámpara

| Estado | Quién lo controla | Idea de ícono |
|--------|-------------------|---------------|
| Full (intensidad máxima) | Teléfono | 🌕 |
| Regulación de intensidad (%) | Teléfono (define el %) | 🎚 |
| Apagado total | Teléfono | 🌑 |
| Automático | La lámpara sola | 🤖 (se muestra distinto) |

### Mapa de temperatura (color de la lámpara)
La lámpara se colorea según la temperatura ambiente: **celeste** cuando hace frío, **rojo** cuando hace calor (degradado intermedio).

### Indicador de conexión
Punto de color + texto "Conectada / Desconectada". Posible implementación como **heartbeat**: si no llega un latido en X segundos, se marca como desconectada.

### Base de datos del histórico
Tabla simple: **`time_day`** (clave), **temperatura**, **humedad**. La lámpara guarda automáticamente (sin acción del usuario). Granularidad por **día** para que el gráfico no quede comprimido.

---

## 5. Consideraciones de diseño a acordar

- **Estado activo**: que quede claro cuál de los 4 está seleccionado y cuál es el Automático (no controlable desde el teléfono).
- **Feedback del POST**: mostrar que el cambio de estado/intensidad se envió.
- **Gráfico de dos escalas**: definir librería (Microcharts `LineChart` o dibujo propio) y cómo se ven las dos escalas.
- **Orientación**: soportar horizontal en el histórico.
- **Estados vacíos y de carga**: qué se ve sin conexión o mientras llega el dato.
- **Paleta y tipografía**.

---

## 6. Actividades concretas (para hacer con el profesor)

| # | Actividad | Entregable |
|---|-----------|------------|
| 1 | Diseñar los 4 estados y el slider de intensidad | Mockup del control |
| 2 | Diseñar el mapa de temperatura (color de la lámpara) | Muestra de color |
| 3 | Diseñar el indicador de conexión (heartbeat) | Componente en el mockup |
| 4 | Diseñar la comparación interna vs externa | Mockup de tiempo real |
| 5 | Diseñar el histórico (calendario + gráfico de dos líneas/dos escalas, horizontal) | Mockup del histórico |
| 6 | Pasar la pantalla principal a XAML | Pantalla en XAML |

---

*Programación III — Aplicaciones Móviles con .NET MAUI — Ciclo Lectivo 2026*
