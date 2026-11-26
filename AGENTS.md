# Proyecto Lámpara 2026 — reglas para agentes

Proyecto académico (Programación III — Aplicaciones Móviles con .NET MAUI, 2026). El
sistema es una **lámpara LED con sensor de temperatura/humedad (ESP32)** que publica
mediciones a una API; la aplicación móvil **LamparaApp** consume esa API para mostrar
el ambiente y controlar la luz. El `README.md` raíz es el mapa del proyecto.

## Alcance

- Lo que se desarrolla en esta materia es la **app móvil** (`src/LamparaApp/`).
- El **firmware ESP32** (`firmware/`) y la **API** (`api/`) se desarrollan en **otra
  materia**: se incluyen como contexto, pero no se documentan ni modifican aquí sin
  información confirmada por el alumno.

## Reglas mínimas

- Trabaja y documenta en español, con tono académico.
- No inventes información: si algo falta (contrato de la API, estados, URL), pregunta
  antes de asumir.
- Usa los nombres reales del proyecto (LamparaApp, LamparaApi), nunca genéricos.
- Enlaza archivos con rutas relativas.
- Git: un commit por trabajo terminado y aprobado por el usuario; nunca hagas push (el
  push es exclusivo del responsable del repositorio). Mensajes en presente y al grano.
- No modifiques código sin autorización explícita. Nunca publiques tokens, contraseñas
  ni secretos (URL/claves de la API van en `appsettings.Development.json`, ignorado).

## Documentación de referencia

- `docs/LamparaApp/Ficha_Relevamiento.md` — qué hace la app, pantallas, entidades, API.
- `docs/LamparaApp/Actividades_Diseño_App.md` — diseño de pantallas y elementos.
- `docs/LamparaApp/Ejercicios_Por_Pantalla.md` — ejercicios por concepto (base de `ejercicios/`).
