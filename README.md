# Proyecto Lámpara 2026 (Lámpara LED)

**Proyecto Lámpara 2026** es un proyecto académico que combina una **lámpara LED con
un microcontrolador ESP32** y un **sensor de temperatura y humedad** con una
**aplicación móvil**. El ESP32 mide las condiciones del ambiente y controla el panel
LED; publica las mediciones a una API, y la app **LamparaApp** permite ver la
temperatura y la humedad del ambiente y **controlar el estado de la luz** desde el
teléfono.

## Contexto

El proyecto se desarrolla en el marco de **Programación III — Aplicaciones Móviles con
.NET MAUI** (ciclo lectivo 2026). El trabajo del alumno en esta materia es la
**aplicación móvil**; el firmware del ESP32 y la API se desarrollan en el marco de
**otra materia** y se incluyen aquí solo como contexto del proyecto integral.

El problema que resuelve: monitorear las condiciones del ambiente (temperatura y
humedad) y controlar la iluminación de la lámpara de forma remota desde el celular.

## Descripción general

El sistema funciona como una cadena de medición, publicación y control:

1. El **ESP32** mide temperatura y humedad del ambiente y controla el **panel LED**.
2. El ESP32 publica las mediciones en la **API (LamparaApi)**.
3. **LamparaApp** consume la API: lee temperatura/humedad y estado (GET) y envía el
   estado/intensidad de la luz elegido por el usuario (POST).

```text
Sensor + panel LED
        │
        ▼
      ESP32  ──publica──►  LamparaApi  ◀──GET / POST──►  LamparaApp (móvil)
                               │
                               ▼
                         base de datos
```

> El estado **Automático** de la luz lo gestiona la propia lámpara; los demás estados
> los controla el usuario desde la app.

## Componentes

### LamparaApp — Aplicación móvil

Aplicación móvil (materia Aplicaciones Móviles / Programación III) que informa el clima
del ambiente medido por la lámpara y permite controlar la luz.

- **Tecnología:** .NET MAUI (C#).
- **Pantallas:**
  - *Principal*: indicador de conexión, lámpara coloreada según la temperatura
    (celeste = frío → rojo = calor), temperatura y humedad del ambiente, comparación
    interna (lámpara) vs externa (clima), 4 estados de luz y slider de intensidad.
  - *Histórico*: calendario para elegir el día y un gráfico de dos líneas (temperatura
    y humedad) con dos escalas y el eje X por hora.
- **Estados de la luz:** Full, Regulación de intensidad (%), Apagado (los tres desde el
  teléfono) y Automático (lo gestiona la lámpara).
- **Relación con la API:** consume temperatura/humedad y estado (GET) y envía el
  estado/intensidad de la luz (POST).
- **Ubicación:** `src/LamparaApp/` (código) y `docs/LamparaApp/` (relevamiento, diseño
  y ejercicios). Prácticas previas en `ejercicios/`.
- **Estado:** en desarrollo (etapa de ejercicios; app sin iniciar).

### Firmware ESP32 — Lámpara y sensor

Firmware del microcontrolador ESP32 que lee el sensor de temperatura/humedad, controla
el panel LED y publica las mediciones a la API.

- **Alcance:** se desarrolla en **otra materia**; se incluye como contexto.
- **Ubicación:** `firmware/`.
- **Estado:** fuera del alcance de esta materia; pendiente de documentar aquí.

### LamparaApi — API REST

API que recibe las mediciones del ESP32, las almacena y las expone a la app.

- **Responsabilidad:** entregar temperatura/humedad y estado actual (GET) y recibir el
  estado/intensidad de la luz (POST).
- **Alcance:** se desarrolla en **otra materia**; se incluye como contexto.
- **Ubicación:** `api/`.
- **Estado:** fuera del alcance de esta materia; contrato (URL, JSON) pendiente.

## Modelo de datos (app)

| Entidad | Campos |
| ------- | ------ |
| `Medicion` | `time_day` (clave), `temperatura`, `humedad` |
| `EstadoLuz` | `estado` (Full / Regulacion / Apagado / Automatico), `intensidad` (%) |

El histórico usa una tabla simple (`time_day`, `temperatura`, `humedad`); la lámpara
guarda las mediciones automáticamente.

## Tecnologías

| Componente | Tecnologías |
| ---------- | ----------- |
| LamparaApp (APP) | .NET MAUI, C# |
| Firmware ESP32 | ESP32 (otra materia) |
| LamparaApi | API REST (otra materia) |

## Estructura del repositorio

```text
Proyecto_Lampara2026/
├── README.md           → este documento: mapa general del proyecto
├── AGENTS.md           → reglas para el agente de IA de desarrollo
├── .gitignore
├── docs/               → documentación del proyecto
│   ├── Documentacion_de_proyecto_2026(Lampara_Led).docx
│   ├── proyectolapmara2026.drawio   → diagrama de arquitectura
│   ├── LamparaApp/     → relevamiento, diseño y ejercicios de la app
│   ├── mockups/        → prototipos de pantallas
│   └── capturas/       → screenshots de la app funcionando
├── ejercicios/         → prácticas pequeñas previas (una carpeta por ejercicio)
├── src/                → código de la app MAUI (LamparaApp)
├── firmware/           → firmware ESP32 (otra materia)
└── api/                → API REST LamparaApi (otra materia)
```

## Documentación

General:

- [Diagrama del proyecto (draw.io)](docs/proyectolapmara2026.drawio) — arquitectura y relaciones entre componentes.
- [Documentación del proyecto (.docx)](docs/Documentacion_de_proyecto_2026%28Lampara_Led%29.docx)

Aplicación (`docs/LamparaApp/`):

- [Ficha de relevamiento](docs/LamparaApp/Ficha_Relevamiento.md) — qué hace la app, pantallas, entidades y API.
- [Actividades de diseño de la app](docs/LamparaApp/Actividades_Diseño_App.md) — diseño de pantallas y elementos.
- [Ejercicios por pantalla](docs/LamparaApp/Ejercicios_Por_Pantalla.md) — práctica por concepto.
- [Prototipo de pantallas](docs/LamparaApp/Prototipo_Pantallas.html) — maqueta visual.

## Ejecución

**LamparaApp (APP)** — en desarrollo (etapa de ejercicios). Al crear el proyecto MAUI
se abrirá la solución en Visual Studio y se ejecutará, o se compilará con
`dotnet build` (SDK de .NET).

**Firmware ESP32** y **LamparaApi** — se desarrollan en otra materia; se documentará su
ejecución cuando se incorporen al repositorio.

## Estado del proyecto

- [ ] **LamparaApp** (APP) — en desarrollo: etapa de ejercicios; pantallas sin iniciar
- [ ] **Firmware ESP32** — otra materia; pendiente de incorporar aquí
- [ ] **LamparaApi** — otra materia; contrato (URL, JSON) pendiente

## Git y trabajo en equipo

- `main` se mantiene estable.
- Commits frecuentes y descriptivos, en presente (ej.: `agrega pantalla Principal`,
  `conecta GET de temperatura`).
- El README se mantiene actualizado: es la cara del proyecto.
- No subir archivos generados (`bin/`, `obj/`) ni secretos (usar `.gitignore`).

---

*Programación III — Aplicaciones Móviles con .NET MAUI — Ciclo Lectivo 2026*
