# -*- coding: utf-8 -*-
"""Genera la Documentación Técnica de Arquitectura de Software para LamparaLedAPI (.docx)"""
from docx import Document
from docx.shared import Pt, Cm, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml.ns import qn
from docx.oxml import OxmlElement

OUT = r"C:\Users\ivard\Downloads\Documentación Técnica - Arquitectura de Software - LamparaLedAPI.docx"

doc = Document()

# ---------- Estilos base ----------
normal = doc.styles["Normal"]
normal.font.name = "Calibri"
normal.font.size = Pt(11)
normal.paragraph_format.space_after = Pt(6)
normal.paragraph_format.line_spacing = 1.15

for lvl, size in [("Heading 1", 18), ("Heading 2", 14), ("Heading 3", 12)]:
    st = doc.styles[lvl]
    st.font.name = "Calibri"
    st.font.size = Pt(size)
    st.font.color.rgb = RGBColor(0x1F, 0x3B, 0x63)
    st.font.bold = True

def h1(t): doc.add_heading(t, level=1)
def h2(t): doc.add_heading(t, level=2)
def h3(t): doc.add_heading(t, level=3)
def p(t, bold=False, italic=False):
    par = doc.add_paragraph()
    run = par.add_run(t)
    run.bold = bold
    run.italic = italic
    return par

def bullet(t, bold_prefix=None):
    par = doc.add_paragraph(style="List Bullet")
    if bold_prefix:
        r = par.add_run(bold_prefix)
        r.bold = True
    par.add_run(t)
    return par

def code(t):
    par = doc.add_paragraph()
    run = par.add_run(t)
    run.font.name = "Consolas"
    run.font.size = Pt(9.5)
    par.paragraph_format.left_indent = Cm(0.75)
    par.paragraph_format.space_after = Pt(4)
    return par

def table(headers, rows, widths=None):
    tb = doc.add_table(rows=1, cols=len(headers))
    tb.style = "Table Grid"
    tb.alignment = WD_TABLE_ALIGNMENT.CENTER
    hdr = tb.rows[0].cells
    for i, htxt in enumerate(headers):
        hdr[i].text = ""
        r = hdr[i].paragraphs[0].add_run(htxt)
        r.bold = True
        r.font.size = Pt(9.5)
    for row in rows:
        cells = tb.add_row().cells
        for i, val in enumerate(row):
            cells[i].text = ""
            r = cells[i].paragraphs[0].add_run(str(val))
            r.font.size = Pt(9.5)
    if widths:
        for i, w in enumerate(widths):
            for row in tb.rows:
                row.cells[i].width = Cm(w)
    doc.add_paragraph()
    return tb

# =====================================================================
# PORTADA
# =====================================================================
for _ in range(6):
    doc.add_paragraph()
t = doc.add_paragraph()
t.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = t.add_run("Documentación\nTécnica")
r.bold = True
r.font.size = Pt(30)
r.font.color.rgb = RGBColor(0x1F, 0x3B, 0x63)

t = doc.add_paragraph()
t.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = t.add_run("Proyecto: API REST LamparaLedAPI (.NET 10 / ASP.NET Core)")
r.font.size = Pt(16)

info = [
    ("Versión:", "1.0"),
    ("Fecha:", "Noviembre 2026"),
    ("Arquitectura:", "Capas (Presentación → Negocio → Infraestructura)"),
    ("Alcance:", "Backend / Web API"),
]
for _ in range(2):
    doc.add_paragraph()
for k, v in info:
    par = doc.add_paragraph()
    par.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r1 = par.add_run(k + " ")
    r1.bold = True
    par.add_run(v)

doc.add_page_break()

# =====================================================================
# 1. INTRODUCCIÓN
# =====================================================================
h1("1. Introducción")

h2("1.1 Propósito")
p("El presente documento tiene como objetivo detallar la arquitectura de "
  "software, el diseño lógico, el flujo de procesos, el entorno de desarrollo "
  "y las consideraciones técnicas y de seguridad del backend (Web API) "
  "desarrollado para el proyecto Lámpara LED Inteligente con Telemetría "
  "Ambiental. La solución de software documentada es LamparaLedAPI, una API "
  "REST construida con ASP.NET Core sobre .NET 10 que actúa como intermediaria "
  "entre el dispositivo físico ESP32-C3 (que hospeda la lámpara LED, el sensor "
  "DHT11 y el servidor HTTP embebido) y los clientes consumidores de la "
  "información (aplicación móvil, tableros de monitoreo o cualquier cliente "
  "HTTP).")
p("Esta documentación está dirigida principalmente al docente evaluador, con "
  "el fin de proporcionar una visión clara y estructurada sobre cómo se "
  "integran las capas de presentación, negocio e infraestructura; el modelo "
  "de datos persistido en PostgreSQL; la comunicación con el dispositivo "
  "ESP32 y con servicios meteorológicos externos; y el ciclo de vida de una "
  "lectura de telemetría desde su captura hasta su almacenamiento y consulta. "
  "Sirve además como guía de referencia para futuras ampliaciones o "
  "mantenimiento del código.")

h2("1.2 Alcance")
p("El alcance de este documento abarca exclusivamente la API REST "
  "LamparaLedAPI, incluyendo:")
bullet("La capa de presentación HTTP: controladores, pipeline de ASP.NET Core, "
       "inyección de dependencias, servicio en segundo plano "
       "(ESP32BackgroundService) y exposición de la especificación OpenAPI.",
       bold_prefix="● ")
bullet("La capa de negocio: orquestación de casos de uso (recepción y "
       "procesamiento de lecturas, consulta remota al ESP32, cambio de modo "
       "de iluminación y consulta de datos históricos), servicios de clima y "
       "objetos de transferencia de datos (DTO).",
       bold_prefix="● ")
bullet("La capa de infraestructura: persistencia con Entity Framework Core "
       "sobre PostgreSQL (Npgsql), cliente HTTP hacia el ESP32, conector hacia "
       "la API externa WeatherAPI y migraciones de base de datos.",
       bold_prefix="● ")
bullet("Los contratos REST expuestos, el modelo de datos relacional, los "
       "requisitos funcionales y no funcionales, las vistas de arquitectura, "
       "lógica, procesos, desarrollo, despliegue y seguridad.",
       bold_prefix="● ")
p("Queda fuera del alcance el firmware residente en el microcontrolador "
  "ESP32-C3 (documentado por separado en la documentación técnica del "
  "dispositivo), el motor de base de datos PostgreSQL en sí, el servicio "
  "externo WeatherAPI y las aplicaciones cliente que consumen esta API.")

# =====================================================================
# 2. VISIÓN GENERAL DEL SISTEMA
# =====================================================================
h1("2. Visión General del Sistema")

h2("2.1 Descripción General")
p("LamparaLedAPI es una aplicación web ASP.NET Core que centraliza la "
  "telemetría ambiental generada por la lámpara LED inteligente. El sistema "
  "opera en una red local (LAN) y cumple dos funciones principales:")
bullet("Persistir y servir el historial de lecturas del dispositivo: cada "
       "registro combina la telemetría capturada por el sensor local del "
       "ESP32 (temperatura y humedad del DHT11, estado del modo de "
       "iluminación) con el clima meteorológico de referencia obtenido desde "
       "la API pública WeatherAPI para la ubicación configurada.",
       bold_prefix="● ")
bullet("Actuar como intermediario de control: permite a los clientes "
       "consultar la última lectura o todo el historial, y reenviar órdenes "
       "de cambio de modo de iluminación hacia el servidor HTTP embebido del "
       "ESP32.",
       bold_prefix="● ")
p("La recolección de datos hacia el backend se realiza por dos vías "
  "complementarias y no excluyentes: (a) el propio ESP32 puede empujar sus "
  "lecturas mediante una petición POST con payload JSON, y (b) un servicio "
  "en segundo plano (ESP32BackgroundService) consulta periódicamente al "
  "dispositivo mediante GET /api/v1/lampara cada intervalo configurable "
  "(por defecto 30 minutos). En ambos casos el pipeline de procesamiento es "
  "el mismo: se enriquece la lectura con datos meteorológicos externos y se "
  "persiste en PostgreSQL.")

h2("2.2 Contexto del Sistema")
p("El sistema interactúa con su entorno a través de interfaces HTTP dentro "
  "de una misma red de área local (LAN) y con un servicio en la nube:")
table(
    ["Actor / Sistema", "Dirección / Ubicación", "Interacción"],
    [
        ["Dispositivo ESP32-C3 (Lámpara LED)", "ESP32:BaseUrl\n(p.ej. http://192.168.1.102)",
         "Expone GET /api/v1/lampara (telemetría JSON) y GET /api/v1/lampara/modo?id={1..4} (cambio de modo). También puede hacer POST /api/v1/Lampara hacia la API."],
        ["LamparaLedAPI (.NET 10)", "http://0.0.0.0:5000 (todas las interfaces)",
         "API REST consumida por clientes; expone endpoints /api/v1/Lampara/*."],
        ["PostgreSQL", "localhost:5432 / base lamparadb",
         "Persistencia relacional de lecturas (tabla lampara_lecturas) mediante EF Core + Npgsql."],
        ["WeatherAPI (weatherapi.com)", "api.weatherapi.com (Internet)",
         "GET /v1/current.json?key={apiKey}&q={ubicacion}&aqi=no → temperatura y humedad de referencia."],
        ["Cliente móvil / consumidor", "Cualquier dispositivo en la LAN",
         "GET /lecturas, GET /actual, POST /modo, GET /consultar."],
    ],
    widths=[4.6, 4.2, 8.0],
)

h2("2.3 Requisitos Funcionales y No Funcionales")
h3("Requisitos Funcionales (RF)")
table(
    ["ID", "Descripción"],
    [
        ["RF01", "Recepción de telemetría: el sistema debe aceptar peticiones POST /api/v1/Lampara con un cuerpo JSON proveniente del ESP32 (fecha_hora, dispositivo, temperatura, humedad, lampara) y persistirla en la base de datos."],
        ["RF02", "Enriquecimiento meteorológico: al procesar cada lectura, el sistema debe consultar la API WeatherAPI para la ubicación configurada (WeatherAPI:Ubicacion) y almacenar la temperatura y humedad de referencia junto a las del sensor. Si la consulta falla, debe guardar la lectura igualmente con valores por defecto."],
        ["RF03", "Consulta periódica al dispositivo: el sistema debe ejecutar un BackgroundService que consulte al ESP32 cada ESP32:IntervaloMinutos (por defecto 30) mediante GET {BaseUrl}/api/v1/lampara, procesando la respuesta con el mismo pipeline de RF01/RF02."],
        ["RF04", "Consulta de lecturas: el sistema debe exponer GET /api/v1/Lampara/lecturas (todas, ordenadas por fecha descendente) y GET /api/v1/Lampara/actual (última lectura disponible, 404 si no hay datos)."],
        ["RF05", "Control remoto de modos: el sistema debe procesar POST /api/v1/Lampara/modo con cuerpo { \"modoId\": entero } y reenviar la orden al ESP32 mediante GET {BaseUrl}/api/v1/lampara/modo?id={modoId}."],
        ["RF06", "Consulta forzada: el sistema debe exponer GET /api/v1/Lampara/consultar para disparar manualmente una consulta al ESP32 y persistir el resultado."],
        ["RF07", "Disponibilidad en LAN: la API debe escuchar en todas las interfaces de red (0.0.0.0:5000) para que dispositivos de la red local la alcancen sin configuración adicional."],
    ],
    widths=[1.6, 15.2],
)

h3("Requisitos No Funcionales (RNF)")
table(
    ["ID", "Descripción"],
    [
        ["RNF01", "Arquitectura en capas con inversión de dependencias: la solución se organiza en tres proyectos (Presentacion, Negocio, Infraestructura) comunicados exclusivamente a través de interfaces, garantizando bajo acoplamiento y testeabilidad."],
        ["RNF02", "Programación asíncrona integral: todos los casos de uso y accesos a I/O (HTTP, base de datos) se implementan con async/await y Task para no bloquear el thread pool."],
        ["RNF03", "Inyección de dependencias: los servicios se registran en el contenedor de DI de ASP.NET Core (AddScoped, AddHttpClient, AddHostedService) y se resuelven por constructor."],
        ["RNF04", "Configuración externalizada: cadenas de conexión, URL del ESP32, intervalo de sondeo y credenciales de WeatherAPI se leen desde appsettings.json / variables de entorno / User Secrets, sin valores sensibles hardcodeados (se provee appsettings.example.json)."],
        ["RNF05", "Migraciones versionadas: el esquema de base de datos se gestiona con migraciones de Entity Framework Core, permitiendo evolución controlada del modelo."],
        ["RNF06", "Tolerancia a fallos de integraciones: si WeatherAPI no responde, la lectura se persiste con valores por defecto (0); los errores del ESP32 se capturan y loguean sin detener el BackgroundService."],
        ["RNF07", "Documentación de API: la aplicación expone la especificación OpenAPI (Swagger) en entorno de desarrollo para facilitar la integración de clientes."],
    ],
    widths=[1.6, 15.2],
)

# =====================================================================
# 3. VISTA DE ARQUITECTURA
# =====================================================================
doc.add_page_break()
h1("3. Vista de Arquitectura")

h2("3.1 Plano de Software (Capas)")
p("La solución sigue una arquitectura en capas estilo Clean Architecture "
  "simplificada, materializada en tres proyectos de .NET 10 dentro de la "
  "solución LamparaLedAPI.slnx:")
table(
    ["Capa", "Proyecto", "Responsabilidad", "Componentes principales"],
    [
        ["Presentación", "Presentacion", "Punto de entrada HTTP y hosting",
         "LamparaController, WeatherForecastController, Program.cs (pipeline, DI, escucha en :5000), ESP32BackgroundService"],
        ["Negocio", "Negocio", "Casos de uso y orquestación",
         "LamparaService (ILamparaService), ClimaService (Negocio.Clima.IClimaService), Meteorologia (IMeteorologia), DTOs: DatosLamparaDTO, ClimaDTO, DatosMeteorologiaDTO"],
        ["Infraestructura", "Infraestructura", "Persistencia y clientes externos",
         "LamparaDbContext (EF Core + Npgsql), LamparaRepository (ILamparaRepository), LamparaEntity, LamparaDTO, ESP32Client (IESP32Client), ConectorWeatherAPI (IConnectorMeteorologiaAPI), migraciones"],
    ],
    widths=[2.4, 2.6, 4.6, 7.2],
)
p("Regla de dependencia: Presentacion referencia a Negocio e "
  "Infraestructura; Negocio referencia a Infraestructura; la comunicación "
  "entre capas se realiza mediante interfaces (ILamparaService, "
  "IClimaService, ILamparaRepository, IESP32Client, "
  "IConnectorMeteorologiaAPI), de modo que la capa superior nunca conoce los "
  "detalles de implementación de la inferior.", bold=False)

h2("3.2 Diagrama de Arquitectura")
p("El siguiente esquema resume los componentes y sus interacciones:")
code(
"┌─────────────────────────── CLIENTES (LAN) ───────────────────────────┐\n"
"│   App móvil / curl / Postman                                         │\n"
"└───────────────▲───────────────────────────────────┬─────────────────┘\n"
"                │ HTTP REST :5000                    │ HTTP REST :5000\n"
"┌───────────────┴───────────────────────────────────▼─────────────────┐\n"
"│ PRESENTACIÓN (ASP.NET Core, Program.cs)                              │\n"
"│   LamparaController ──► ILamparaService        ESP32BackgroundService │\n"
"│   (endpoints REST)        ▲                          │ (cada 30 min)   │\n"
"└───────────────────────────┼──────────────────────────┼─────────────────┘\n"
"                            │                          │\n"
"┌───────────────────────────▼──────────────────────────▼─────────────────┐\n"
"│ NEGOCIO                                                          │\n"
"│   LamparaService ──► ILamparaRepository      IClimaService         │\n"
"│   (orquestación)        ▲                          │               │\n"
"└──────────────────────────┼──────────────────────────┼───────────────┘\n"
"                           │                          │\n"
"┌──────────────────────────▼──────────┐  ┌─────────────▼──────────────┐\n"
"│ INFRAESTRUCTURA                      │  │  ClimaService (Negocio)    │\n"
"│   LamparaRepository ──► LamparaDbContext (EF Core)                    │\n"
"│        │                     │                                     │\n"
"│   LamparaEntity          PostgreSQL (Npgsql)   WeatherAPI HTTP      │\n"
"│                                                     │               │\n"
"│   ESP32Client ──HTTP──► ESP32 :80 /api/v1/lampara   │               │\n"
"└─────────────────────────────────────────────────────┴───────────────┘\n"
)
p("Flujo de datos principal: el ESP32BackgroundService (o un POST directo "
  "del ESP32) dispara LamparaService.ConsultarESP32Async / ProcesarDatosLampara; "
  "este obtiene la telemetría del dispositivo (o la recibe por body), la "
  "enriquece con WeatherAPI y delega la persistencia en LamparaRepository, "
  "que inserta una fila en la tabla lampara_lecturas de PostgreSQL. Los "
  "clientes consultan los datos mediante los endpoints GET expuestos por "
  "LamparaController.")

# =====================================================================
# 4. VISTA LÓGICA
# =====================================================================
doc.add_page_break()
h1("4. Vista Lógica")

h2("4.1 Diagrama de Clases (Objetos y Estructuras Principales)")
p("La API está implementada en C# bajo el paradigma orientado a objetos con "
  "inyección de dependencias. Las estructuras principales son:")
table(
    ["Tipo", "Capa / Proyecto", "Miembros clave", "Rol"],
    [
        ["LamparaController", "Presentacion.Controllers", "RecibirDatosLampara (POST), ConsultarESP32 (GET consultar), CambiarModoLampara (POST modo), ObtenerTodasLecturas (GET lecturas), ObtenerLecturaActual (GET actual)", "Controlador REST; único punto de entrada HTTP; traduce requests a llamadas de ILamparaService."],
        ["ModoRequest", "Presentacion.Controllers", "ModoId (int)", "DTO de entrada para el cambio de modo."],
        ["ESP32BackgroundService", "Presentacion", "ExecuteAsync", "BackgroundService que sondea al ESP32 cada IntervaloMinutos creando un scope de DI por iteración."],
        ["ILamparaService / LamparaService", "Negocio.Lampara", "ProcesarDatosLampara, ConsultarESP32Async, CambiarModoLamparaAsync, ObtenerTodasLecturas, ObtenerUltimaLectura", "Orquesta el caso de uso: combina ESP32Client, IClimaService y ILamparaRepository."],
        ["IClimaService / ClimaService (Negocio.Clima)", "Negocio.Clima", "ObtenerClimaActual(ubicacion)", "Consulta WeatherAPI y devuelve ClimaDTO (Temperatura, Humedad)."],
        ["IMeteorologia / Meteorologia", "Negocio.Clima", "ObtenerMeteorologiaActual(lat, long)", "Capa de dominio alternativa que delega en IConnectorMeteorologiaAPI y mapea a DatosMeteorologiaDTO."],
        ["ILamparaRepository / LamparaRepository", "Infraestructura.Lampara", "GuardarLectura, ObtenerTodasLecturas, ObtenerUltimaLectura", "Acceso a datos vía LamparaDbContext (EF Core)."],
        ["IESP32Client / ESP32Client", "Infraestructura.Lampara", "ObtenerDatosLamparaAsync, CambiarModoLamparaAsync", "Cliente HTTP tipado hacia el servidor embebido del ESP32."],
        ["IConnectorMeteorologiaAPI / ConectorWeatherAPI", "Infraestructura.WeatherAPI / Meteorologia", "ObtenerDatosMeteorologia(lat, long)", "Implementación alternativa de conector meteorológico por coordenadas."],
        ["LamparaDbContext", "Infraestructura.Lampara", "DbSet<LamparaEntity> LamparaLecturas, OnModelCreating", "Contexto EF Core configurado con Npgsql; define claves, tipos y longitudes."],
        ["LamparaEntity", "Infraestructura.Lampara", "Id, FechaHora, DispositivoId, TemperaturaSensor, HumedadSensor, TemperaturaApi, HumedadApi, EstadoLampara", "Entidad persistida ([Table(\"lampara_lecturas\")])."],
        ["LamparaDTO (Infraestructura)", "Infraestructura.Lampara", "Propiedades con [JsonPropertyName], Convertir()", "DTO de entrada del JSON del ESP32; convierte a LamparaEntity parseando fecha_hora."],
        ["DatosLamparaDTO", "Negocio.Lampara", "Id, FechaHora, DispositivoId, TemperaturaSensor, HumedadSensor, TemperaturaApi, HumedadApi, EstadoLampara", "DTO de salida hacia los clientes."],
    ],
    widths=[3.6, 3.2, 5.4, 4.6],
)

h2("4.2 Modelado de Datos")
p("La API persiste la telemetría en PostgreSQL mediante Entity Framework "
  "Core 9 (proveedor Npgsql). El esquema vigente (tras la migración "
  "ConsolidateTables) contiene una única tabla consolidada:")
p("Tabla lampara_lecturas:", bold=True)
table(
    ["Columna", "Tipo PostgreSQL", "Nullable", "Descripción"],
    [
        ["Id", "integer (IDENTITY)", "No", "Clave primaria autoincremental."],
        ["FechaHora", "timestamp without time zone", "No", "Fecha y hora de la lectura reportada por el dispositivo (parseada desde fecha_hora; fallback DateTime.Now)."],
        ["DispositivoId", "character varying(100)", "No", "Identificador del dispositivo (campo JSON \"dispositivo\", p.ej. ESP32_Lampara)."],
        ["TemperaturaSensor", "double precision", "No", "Temperatura ambiente medida por el DHT11 del ESP32 (°C)."],
        ["HumedadSensor", "double precision", "No", "Humedad relativa medida por el sensor del ESP32 (%)."],
        ["TemperaturaApi", "double precision", "No", "Temperatura de referencia obtenida de WeatherAPI (°C); 0 si la consulta falla."],
        ["HumedadApi", "double precision", "No", "Humedad de referencia obtenida de WeatherAPI (%); 0 si la consulta falla."],
        ["EstadoLampara", "character varying(10)", "No", "Estado/modo de iluminación reportado (campo JSON \"lampara\", valores 1..4 como cadena)."],
    ],
    widths=[3.4, 4.0, 1.8, 8.0],
)
p("Historial de migraciones EF Core aplicadas (Infraestructura/Migrations): "
  "InitialCreate, AddClimaTabla, ChangeToTimestampWithoutTimeZone y "
  "ConsolidateTables (esta última eliminó la tabla clima_lecturas, renombró "
  "columnas y agregó TemperaturaApi/HumedadApi para consolidar ambos flujos de "
  "datos en una sola tabla).")

h2("4.3 Descripción de las Relaciones entre Componentes")
bullet("Separación de responsabilidades: LamparaController no contiene lógica "
       "de negocio; solo valida el request y delega en ILamparaService. "
       "LamparaService no conoce HTTP ni SQL: consume abstracciones "
       "(IESP32Client, IClimaService, ILamparaRepository).",
       bold_prefix="● ")
bullet("Inyección de dependencias: Program.cs registra "
       "ILamparaService→LamparaService (Scoped), IClimaService→ClimaService "
       "(HttpClient), ILamparaRepository→LamparaRepository (Scoped), "
       "IESP32Client→ESP32Client (HttpClient tipado) y "
       "ESP32BackgroundService como hosted service.",
       bold_prefix="● ")
bullet("Doble vía de ingesta, pipeline único: tanto el POST directo del ESP32 "
       "como el sondeo periódico del BackgroundService convergen en "
       "ProcesarDatosLampara(string json), garantizando que toda lectura "
       "reciba el mismo enriquecimiento meteorológico y la misma "
       "persistencia.",
       bold_prefix="● ")
bullet("Resiliencia del flujo: la consulta a WeatherAPI está envuelta en "
       "try/catch; un fallo externo degrada los campos TemperaturaApi/"
       "HumedadApi a 0 sin impedir el guardado. El BackgroundService captura "
       "excepciones por iteración y continúa el ciclo, y crea un scope de DI "
       "nuevo en cada sondeo para evitar consumo de recursos acumulado.",
       bold_prefix="● ")
bullet("Mapeo JSON↔Dominio: LamparaDTO usa [JsonPropertyName] para traducir "
       "el snake_case del firmware (fecha_hora, dispositivo, temperatura, "
       "humedad, lampara) y Convertir() normaliza la fecha (DateTime.TryParse "
       "con SpecifyKind Local y fallback a DateTime.Now).",
       bold_prefix="● ")

# =====================================================================
# 5. VISTA DE PROCESOS
# =====================================================================
doc.add_page_break()
h1("5. Vista de Procesos")

h2("5.1 Flujo de Trabajo")
p("La aplicación sigue el modelo de hosting de ASP.NET Core: una "
  "inicialización única (Program.cs) seguida de un pipeline de peticiones "
  "HTTP concurrente y un ciclo de vida del BackgroundService:")
code(
"Program.cs (arranque):\n"
"  1. WebApplication.CreateBuilder(args)\n"
"  2. AddControllers() + AddOpenApi()\n"
"  3. AddDbContext<LamparaDbContext>(UseNpgsql(DefaultConnection))\n"
"  4. AddHttpClient() + registro de servicios (Scoped / typed clients)\n"
"  5. AddHostedService<ESP32BackgroundService>()\n"
"  6. app.Run(\"http://0.0.0.0:5000\")  → escucha en todas las interfaces\n"
"\n"
"Ciclo de vida del BackgroundService (ExecuteAsync):\n"
"  loop infinito:\n"
"     a. Crear scope de DI (IServiceProvider.CreateScope)\n"
"     b. Resolver ILamparaService y ejecutar ConsultarESP32Async()\n"
"     c. Loguear éxito o error por consola\n"
"     d. Task.Delay(IntervaloMinutos) con CancellationToken\n"
"\n"
"Pipeline de una lectura (ProcesarDatosLampara):\n"
"  JSON → LamparaDTO.Deserialize → ClimaService.ObtenerClimaActual\n"
"       → DTO.Convertir(tempApi, humApi) → LamparaEntity\n"
"       → LamparaRepository.GuardarLectura → SaveChangesAsync (PostgreSQL)\n"
)

h2("5.2 Diagramas de Secuencia")
h3("Escenario A: Sondeo periódico del ESP32 (BackgroundService)")
code(
"ESP32BackgroundService   ILamparaService      ESP32Client      WeatherAPI      PostgreSQL\n"
"        │                     │                  │                │               │\n"
"  ──► cada IntervaloMinutos ─►│                  │                │               │\n"
"        │──ConsultarESP32Async►│                  │                │               │\n"
"        │                     │──ObtenerDatosLamparaAsync──────────►│               │\n"
"        │                     │◄─GET {BaseUrl}/api/v1/lampara───────│               │\n"
"        │                     │   (JSON telemetría)                │               │\n"
"        │                     │──ObtenerClimaActual(ubicacion)────────────────────►│\n"
"        │                     │◄─temp/humedad──────────────────────────────────────│\n"
"        │                     │──GuardarLectura(entity)───────────────────────────►│\n"
"        │                     │   INSERT INTO lampara_lecturas                     │\n"
"        │◄── fin iteración ───│                  │                │               │\n"
)
h3("Escenario B: Consulta y control vía API REST")
code(
"Cliente (app/curl)   LamparaController   ILamparaService   ESP32Client/Repo   Recurso\n"
"        │                   │                 │                  │              │\n"
"GET /lecturas ────────►│                 │                  │              │\n"
"                       │──ObtenerTodasLecturas►│                  │              │\n"
"                       │                 │──repo.ObtenerTodasLecturas()──────►  │\n"
"                       │◄── List<DatosLamparaDTO> ──────────────────│              │\n"
"        │◄── 200 OK (JSON) │                 │                  │              │\n"
"\n"
"POST /modo {modoId} ──►│                 │                  │              │\n"
"                       │──CambiarModoLamparaAsync►│                │              │\n"
"                       │                 │──GET {BaseUrl}/api/v1/lampara/modo?id=N►│\n"
"                       │◄── 200 OK \"Modo cambiado\" ──────────────│              │\n"
"        │◄── 200 OK ──────│                 │                  │              │\n"
)
h3("Escenario C: Ingesta directa (push) desde el ESP32")
code(
"ESP32 firmware        LamparaController        ILamparaService        PostgreSQL\n"
"      │                       │                       │                    │\n"
"POST /api/v1/Lampara ──►│──RecibirDatosLampara──►│                    │\n"
"  body: JSON telemetría  │   (JsonElement)         │──ProcesarDatosLampara│\n"
"                         │                         │──WeatherAPI + repo──►│\n"
"                         │◄── 200 \"Datos guardados correctamente\" ──────│\n"
"                         │   (o 400 si falla la deserialización)          │\n"
)

# =====================================================================
# 6. VISTA DE DESARROLLO
# =====================================================================
doc.add_page_break()
h1("6. Vista de Desarrollo")

h2("6.1 Estructura del Código")
p("El repositorio sigue la estructura estándar de una solución .NET con "
  "tres proyectos y separación por capas:")
code(
"api/\n"
"├── README.md\n"
"└── LamparaLedAPI/\n"
"    ├── LamparaLedAPI.slnx\n"
"    ├── Presentacion/                  # Web API (Microsoft.NET.Sdk.Web)\n"
"    │   ├── Program.cs                 #   Bootstrap, DI, pipeline, :5000\n"
"    │   ├── ESP32BackgroundService.cs  #   Sondeo periódico al ESP32\n"
"    │   ├── Controllers/\n"
"    │   │   ├── LamparaController.cs   #   Endpoints /api/v1/Lampara\n"
"    │   │   └── WeatherForecastController.cs\n"
"    │   ├── WeatherForecast.cs\n"
"    │   ├── appsettings.json / .Development.json / .example.json\n"
"    │   └── Properties/launchSettings.json\n"
"    ├── Negocio/                       # Casos de uso (Microsoft.NET.Sdk)\n"
"    │   ├── Lampara/\n"
"    │   │   ├── ILamparaService.cs / LamparaService.cs\n"
"    │   │   ├── IClimaService.cs / ClimaService.cs   (versión Lampara)\n"
"    │   │   ├── DatosLamparaDTO.cs / ClimaDTO.cs\n"
"    │   └── Clima/\n"
"    │       ├── IClimaService.cs / ClimaService.cs   (versión activa)\n"
"    │       ├── IMeteorologia.cs / Meteorologia.cs\n"
"    │       └── ClimaDTO.cs / DatosMeteorologiaDTO.cs\n"
"    └── Infraestructura/               # Datos e integraciones\n"
"        ├── Lampara/\n"
"        │   ├── LamparaDbContext.cs    #   EF Core + Npgsql\n"
"        │   ├── LamparaEntity.cs       #   [Table(\"lampara_lecturas\")]\n"
"        │   ├── LamparaRepository.cs / ILamparaRepository.cs\n"
"        │   ├── ESP32Client.cs / IESP32Client.cs\n"
"        │   ├── LamparaDTO.cs          #   DTO JSON del ESP32 + Convertir()\n"
"        │   └── WeatherAPIResponse.cs\n"
"        ├── Meteorologia/\n"
"        │   ├── IConnectorMeteorologiaAPI.cs / MeteorologiaDTO.cs\n"
"        ├── WeatherAPI/\n"
"        │   ├── ConectorWeatherAPI.cs / WeatherDTO.cs / WeatherAPIResponse.cs\n"
"        ├── DesignTimeDbContextFactory.cs\n"
"        └── Migrations/                # 4 migraciones EF Core\n"
)
p("Nota: coexisten dos implementaciones del servicio de clima "
  "(Negocio.Clima.ClimaService, que es la registrada y utilizada por "
  "LamparaService, y Negocio.Lampara.ClimaService junto con "
  "Meteorologia/ConectorWeatherAPI, una variante por coordenadas). La ruta "
  "activa en producción es Negocio.Clima.IClimaService → ClimaService.")

h2("6.2 Herramientas y Entorno de Desarrollo")
bullet("SDK: .NET 10 (TargetFramework net10.0 en los tres proyectos).",
       bold_prefix="● ")
bullet("IDE: Visual Studio / Visual Studio Code con soporte C#; solución en "
       "formato .slnx.", bold_prefix="● ")
bullet("Framework web: ASP.NET Core (Microsoft.AspNetCore.OpenApi 10.0.12 "
       "para documentación OpenAPI en desarrollo).", bold_prefix="● ")
bullet("Persistencia: Entity Framework Core 9.0.0 + "
       "Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0; dotnet-ef para "
       "migraciones (DesignTimeDbContextFactory incluido).", bold_prefix="● ")
bullet("HTTP: Microsoft.Extensions.Http 9.0.0 (AddHttpClient, clientes "
       "tipados IHttpClientFactory).", bold_prefix="● ")
bullet("Lenguaje: C# con Nullable enable e ImplicitUsings enable en todos "
       "los proyectos.", bold_prefix="● ")
bullet("Ejecución: dotnet run --project Presentacion; la API queda "
       "disponible en http://0.0.0.0:5000 (todas las interfaces).",
       bold_prefix="● ")
bullet("Base de datos: PostgreSQL local, base lamparadb; migraciones con "
       "dotnet ef database update --project Infraestructura "
       "--startup-project Presentacion.", bold_prefix="● ")

h2("6.3 Prácticas de Desarrollo")
bullet("Inyección de dependencias por constructor: ninguna clase instancia "
       "dependencias con new; todo se resuelve desde el contenedor "
       "(ILamparaService, ILamparaRepository, IESP32Client, IClimaService).",
       bold_prefix="● ")
bullet("Programación asíncrona de punta a punta: Task/await en controladores, "
       "servicios, repositorio (SaveChangesAsync) y clientes HTTP "
       "(GetAsync/ReadAsStringAsync).", bold_prefix="● ")
bullet("Separación DTO/Entidad: el JSON del ESP32 se deserializa en "
       "Infraestructura.Lampara.LamparaDTO, se convierte a LamparaEntity para "
       "persistir y se proyecta a Negocio.Lampara.DatosLamparaDTO para la "
       "salida, evitando exponer el modelo de datos directamente.",
       bold_prefix="● ")
bullet("Manejo de errores en frontera: los controladores envuelven la lógica "
       "en try/catch y traducen excepciones a 400 Bad Request; la "
       "ausencia de datos devuelve 404 explícito en /actual.",
       bold_prefix="● ")
bullet("Resiliencia: EnsureSuccessStatusCode en clientes HTTP, try/catch "
       "alrededor de la consulta meteorológica con degradación a valores "
       "por defecto, y captura de excepciones por iteración en el "
       "BackgroundService.", bold_prefix="● ")
bullet("Configuración por ambiente: appsettings.json base + "
       "appsettings.Development.json + appsettings.example.json como "
       "plantilla sin secretos; UserSecretsId configurado en "
       "Presentacion.csproj para credenciales locales.",
       bold_prefix="● ")
bullet("Convenciones de nomenclatura en español para dominio "
       "(ProcesarDatosLampara, ObtenerUltimaLectura, CambiarModoLamparaAsync) "
       "y en inglés para infraestructura .NET.", bold_prefix="● ")

# =====================================================================
# 7. VISTA DE DESPLIEGUE
# =====================================================================
doc.add_page_break()
h1("7. Vista de Despliegue")

h2("7.1 Entorno de Despliegue")
p("La API se despliega como aplicación ASP.NET Core autocontenida o "
  "dependiente del framework en un host dentro de la LAN del proyecto:")
table(
    ["Componente", "Ubicación", "Detalle"],
    [
        ["LamparaLedAPI", "Host local (desarrollo) / servidor LAN", "Escucha en http://0.0.0.0:5000, accesible desde cualquier dispositivo de la red."],
        ["PostgreSQL", "Mismo host o servidor de red", "Puerto 5432; base de datos lamparadb; una tabla lampara_lecturas."],
        ["ESP32-C3", "Dispositivo físico en la LAN", "Servidor HTTP embebido en puerto 80; IP configurable vía ESP32:BaseUrl."],
        ["WeatherAPI", "api.weatherapi.com (Internet)", "Requiere salida a Internet y WeatherAPI:ApiKey válida."],
    ],
    widths=[3.4, 4.6, 9.0],
)

h2("7.2 Configuración")
p("La configuración se resuelve por la jerarquía estándar de ASP.NET Core "
  "(appsettings.json → appsettings.{Environment}.json → variables de "
  "entorno / User Secrets):")
code(
'{\n'
'  "ConnectionStrings": {\n'
'    "DefaultConnection": "Host=localhost;Port=5432;Database=lamparadb;Username=...;Password=..."\n'
'  },\n'
'  "WeatherAPI": {\n'
'    "ApiKey": "[api-key]",\n'
'    "Ubicacion": "Eduardo Castex, Argentina"\n'
'  },\n'
'  "ESP32": {\n'
'    "BaseUrl": "http://192.168.1.102",\n'
'    "IntervaloMinutos": 30\n'
'  }\n'
'}\n'
)
bullet("ConnectionStrings:DefaultConnection → cadena Npgsql para EF Core.",
       bold_prefix="● ")
bullet("WeatherAPI:ApiKey / WeatherAPI:Ubicacion → credencial y lugar de "
       "consulta meteorológica (usada por ClimaService.ObtenerClimaActual).",
       bold_prefix="● ")
bullet("ESP32:BaseUrl → URL base del firmware del ESP32; "
       "ESP32:IntervaloMinutos → período del sondeo del BackgroundService.",
       bold_prefix="● ")

# =====================================================================
# 8. VISTA DE SEGURIDAD
# =====================================================================
doc.add_page_break()
h1("8. Vista de Seguridad")

h2("8.1 Mecanismos de Autenticación y Autorización")
bullet("Perímetro de red: la API opera dentro de la LAN del proyecto; el "
       "router/switch actúa como primera barrera. No está pensada para "
       "exposición directa a Internet.", bold_prefix="● ")
bullet("Autenticación de aplicación: actualmente no se implementan esquemas "
       "de autenticación (JWT, API Key, Basic Auth) ni políticas de "
       "autorización; el pipeline solo invoca app.UseAuthorization() sin "
       "esquemas configurados. Es una decisión acorde al entorno académico "
       "controlado.", bold_prefix="● ")
bullet("Recomendación para producción: agregar autenticación (por ejemplo "
       "JWT Bearer o API Key middleware), habilitar HTTPS con certificado "
       "y restringir CORS a los orígenes del cliente móvil.",
       bold_prefix="● ")

h2("8.2 Protección de Datos")
bullet("Datos en tránsito: la comunicación con clientes y con el ESP32 usa "
       "HTTP plano (puerto 5000 y 80) sin TLS; la confidencialidad depende "
       "de la confiabilidad de la LAN. La consulta a WeatherAPI también es "
       "HTTP.", bold_prefix="● ")
bullet("Datos en reposo: PostgreSQL almacena telemetría sin cifrado "
       "adicional a nivel de aplicación; no se persisten datos personales.",
       bold_prefix="● ")
bullet("Secretos y credenciales: la cadena de conexión y la API key de "
       "WeatherAPI se externalizan en configuración (appsettings / User "
       "Secrets / variables de entorno). Existe appsettings.example.json "
       "como plantilla y el appsettings.json versionado mantiene los valores "
       "vacíos, evitando filtrar secretos al repositorio.",
       bold_prefix="● ")
bullet("Superficie de ataque acotada: la API expone solo 5 endpoints "
       "funcionales bajo /api/v1/Lampara (más el WeatherForecastController "
       "de plantilla), todos de lectura salvo POST de ingesta y cambio de "
       "modo.", bold_prefix="● ")

h2("8.3 Gestión de Vulnerabilidades")
bullet("Validación de entrada: LamparaController recibe JsonElement y "
       "delega la deserialización a System.Text.Json; un JSON malformado o "
       "nulo produce excepción capturada → HTTP 400. ModoRequest.ModoId es "
       "entero fuertemente tipado.", bold_prefix="● ")
bullet("Inyección SQL: mitigada por diseño al usar Entity Framework Core con "
       "consultas LINQ parametrizadas; no hay concatenación de SQL.",
       bold_prefix="● ")
bullet("Manejo de errores: las excepciones se capturan en los controladores "
       "y en el BackgroundService; no se exponen stack traces al cliente "
       "(solo ex.Message en el cuerpo del 400).",
       bold_prefix="● ")
bullet("Resiliencia ante dependencias externas: EnsureSuccessStatusCode en "
       "los clientes HTTP evita procesar respuestas de error como válidas; "
       "la API key vacía en WeatherAPI genera excepción controlada antes de "
       "la llamada.", bold_prefix="● ")
bullet("Mejoras pendientes recomendadas: agregar autenticación/autorización, "
       "HTTPS, logging estructurado con ILogger (hoy se usa Console.WriteLine "
       "en el BackgroundService), reintentos con Polly para llamadas HTTP, "
       "validación de rango para modoId (1..4) y paginación en /lecturas.",
       bold_prefix="● ")

# =====================================================================
# 9. ANEXOS
# =====================================================================
doc.add_page_break()
h1("9. Anexos")

h2("9.1 Contrato de la API (Endpoints)")
table(
    ["Método", "Ruta", "Entrada", "Salida", "Descripción"],
    [
        ["POST", "/api/v1/Lampara", "JSON telemetría ESP32\n{fecha_hora, dispositivo, temperatura, humedad, lampara}", "200 \"Datos guardados correctamente\" / 400", "Recibe y persiste una lectura (usado por el firmware)."],
        ["GET", "/api/v1/Lampara/consultar", "—", "200 \"Consulta al ESP32 realizada...\" / 400", "Fuerza sondeo al ESP32 y guarda la lectura."],
        ["POST", "/api/v1/Lampara/modo", "{ \"modoId\": 1..4 }", "200 \"Modo de lámpara cambiado...\" / 400", "Reenvía cambio de modo al ESP32."],
        ["GET", "/api/v1/Lampara/lecturas", "—", "200 JSON array / 400", "Devuelve todas las lecturas ordenadas por FechaHora desc."],
        ["GET", "/api/v1/Lampara/actual", "—", "200 JSON objeto / 404 / 400", "Devuelve la última lectura disponible."],
    ],
    widths=[1.6, 4.2, 4.4, 3.2, 3.6],
)
p("Ejemplo de lectura devuelta por /actual (DatosLamparaDTO):")
code(
'{\n'
'  "id": 128,\n'
'  "fechaHora": "2026-11-23T18:30:00",\n'
'  "dispositivoId": "ESP32_Lampara",\n'
'  "temperaturaSensor": 23.5,\n'
'  "humedadSensor": 48.0,\n'
'  "temperaturaApi": 22.1,\n'
'  "humedadApi": 55.0,\n'
'  "estadoLampara": "3"\n'
'}\n'
)

h2("9.2 Documentación de Referencia")
bullet("ASP.NET Core / .NET 10: documentación oficial de Minimal APIs y "
       "controladores, inyección de dependencias y BackgroundService.",
       bold_prefix="● ")
bullet("Entity Framework Core 9 + Npgsql: DbContext, migraciones, "
       "UseNpgsql.", bold_prefix="● ")
bullet("WeatherAPI.com: endpoint current.json (temperatura/humedad actual "
       "por ubicación).", bold_prefix="● ")
bullet("PostgreSQL: tipos timestamp without time zone, double precision, "
       "varchar.", bold_prefix="● ")
bullet("Documentación del firmware asociado: Documentación Técnica "
       "LamparaLedESP32 (contrato JSON del dispositivo).", bold_prefix="● ")

h2("9.3 Glosario")
table(
    ["Término", "Definición"],
    [
        ["API REST", "Interfaz de programación que usa HTTP y recursos para intercambiar datos estructurados (JSON) entre sistemas."],
        ["ASP.NET Core", "Framework web multiplataforma de Microsoft para construir APIs y aplicaciones web en .NET."],
        ["BackgroundService", "Clase de ASP.NET Core que ejecuta una tarea en segundo plano durante el ciclo de vida de la aplicación."],
        ["Clean Architecture / Capas", "Estilo de arquitectura que separa el software en capas (Presentación, Negocio, Infraestructura) con dependencias hacia adentro."],
        ["DbContext", "Clase de Entity Framework Core que representa una sesión con la base de datos y el mapeo de entidades."],
        ["DI (Inyección de Dependencias)", "Patrón que provee las dependencias de un objeto desde un contenedor externo en lugar de crearlas internamente."],
        ["DTO", "Data Transfer Object: objeto simple usado para transferir datos entre capas o hacia/desde la API, sin lógica."],
        ["EF Core", "Entity Framework Core: ORM de .NET para acceso a datos relacionales con migraciones."],
        ["HttpClient tipado", "Cliente HTTP generado por IHttpClientFactory asociado a una interfaz concreta."],
        ["LAN", "Red de Área Local: red privada donde conviven el ESP32, el servidor API y los clientes."],
        ["Npgsql", "Proveedor ADO.NET/EF Core para PostgreSQL."],
        ["OpenAPI", "Especificación estándar para describir APIs REST; habilitada en desarrollo vía MapOpenApi."],
        ["Telemetría", "Datos de medición remotos (temperatura, humedad, estado) enviados por el dispositivo."],
    ],
    widths=[4.2, 12.6],
)

h2("9.4 Registro de Cambios")
table(
    ["Versión", "Fecha", "Autor", "Descripción de los Cambios"],
    [
        ["1.0", "Noviembre 2026", "Estudiante / Desarrollador",
         "Creación inicial completa de la documentación de arquitectura de software basada en el código fuente de LamparaLedAPI (Presentacion, Negocio, Infraestructura; .NET 10, EF Core + PostgreSQL, integración ESP32 y WeatherAPI)."],
    ],
    widths=[1.8, 3.0, 4.0, 8.4],
)

doc.save(OUT)
print("OK ->", OUT)
