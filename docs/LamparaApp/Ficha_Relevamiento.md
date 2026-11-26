# Ficha de Relevamiento de Proyecto

**Programación III — Aplicaciones Móviles con .NET MAUI**  
**Segundo Cuatrimestre 2026 — Proyecto Final Integrador**

---

## 1. Datos del alumno / grupo

| Campo | Detalle |
|-------|---------|
| Alumno/s | Ivar (Rodriguez) |
| Nombre del proyecto | Proyecto Lámpara 2026 (Lámpara LED) |
| Repositorio GitHub | https://github.com/ivards34/Proyecto_Lampara2026 |
| Fecha de relevamiento | *(a completar)* |

---

## 2. Descripción general

**¿Qué hace la app?**

Una lámpara LED con un microcontrolador ESP32 conectado a internet y un sensor que mide temperatura y humedad relativa del ambiente. El ESP32 envía esos datos a una API, y la app permite **ver la temperatura y la humedad** del ambiente, además de **controlar el estado de la luz** de la lámpara (elegir entre tres estados).

**¿Quién la va a usar?**

El usuario que tiene la lámpara en su ambiente y quiere consultar temperatura/humedad y manejar la luz desde el teléfono.

**¿Qué problema resuelve?**

Permite monitorear las condiciones del ambiente (temperatura y humedad) y controlar la iluminación de forma remota desde el celular.

---

## 3. Funcionalidades principales

> **Ajustes acordados**: 4 estados de lámpara; color según temperatura; indicador de conexión; comparación interna vs externa; histórico por día con gráfico de dos líneas.

| # | Funcionalidad | Descripción | Prioridad |
|---|---------------|-------------|-----------|
| 1 | Ver temperatura y humedad en tiempo real | Datos del ambiente (vía API) | Alta |
| 2 | Comparar interna vs externa | Temperatura interna (lámpara) vs externa (clima), estilo Weather App | Alta |
| 3 | Controlar estado de la luz (4 estados) | Full, Regulación %, Apagado (teléfono) + Automático (lámpara) | Alta |
| 4 | Regular intensidad (%) | El usuario define el porcentaje de intensidad | Alta |
| 5 | Color según temperatura | La lámpara se colorea celeste (frío) → rojo (calor) | Media |
| 6 | Indicador de conexión | Muestra si la lámpara está conectada (heartbeat) | Media |
| 7 | Histórico por día | Gráfico de temperatura y humedad por hora, filtrado por fecha | Alta |

---

## 4. Pantallas previstas

| # | Pantalla | ¿Qué se ve / qué se hace? |
|---|----------|----------------------------|
| 1 | Principal | Conexión, lámpara coloreada por temperatura, temp/humedad, comparación interna/externa, 4 estados + slider de intensidad |
| 2 | Histórico | Calendario (selector de día) arriba + gráfico horizontal de dos líneas (temp/humedad) con dos escalas |

---

## 5. Entidades y datos (Modelos)

| Entidad | Campos (nombre y tipo) |
|---------|-------------------------|
| Medicion | time_day (clave), temperatura, humedad |
| EstadoLuz | estado (Full / Regulacion / Apagado / Automatico), intensidad (%) |

> El histórico usa una **tabla simple**: `time_day` (clave), `temperatura`, `humedad`. La lámpara guarda automáticamente.

---

## 6. Conexión con la API (GET / POST)

**¿La app usa una API existente o una propia?**

- [x] API propia (`LamparaApi`, según el diagrama)

**URL de la API**: *(pendiente)*

### Operaciones GET (traer datos)

| Endpoint | ¿Qué datos trae? | ¿En qué pantalla? |
|----------|------------------|-------------------|
| GET | temperatura/humedad actuales + estado + conexión | Principal |
| GET | histórico de un día (por hora) | Histórico |

### Operaciones POST (enviar datos)

| Endpoint | ¿Qué datos envía? | ¿Desde qué pantalla? |
|----------|-------------------|----------------------|
| POST | estado de luz + intensidad (%) | Principal |

> Nota: el ESP32 publica las mediciones a la API. La **app** las lee (GET) y envía el estado/intensidad (POST). El estado Automático lo gestiona la lámpara.

---

## 7. Tecnologías del curso que va a usar

> A marcar con Ivar. Propuesta según el proyecto:

- [x] Navegación entre pantallas (Shell) — si hay más de una pantalla
- [ ] Arquitectura MVVM
- [ ] Base de datos local (SQLite) — si guarda historial
- [x] Consumo de API REST (GET / POST)
- [ ] Cámara / Galería
- [ ] Geolocalización (GPS)
- [ ] Sensores *(el sensor está en el ESP32, no en el teléfono)*
- [ ] Notificaciones
- [ ] Compartir contenido

---

## 8. Notas del profesor

Relevamiento reconstruido a partir del `.docx` y el diagrama draw.io del repo. Ver `Analisis_y_Seguimiento.md` para observaciones y qué falta definir.

---

*Programación III — Aplicaciones Móviles con .NET MAUI — Ciclo Lectivo 2026*
