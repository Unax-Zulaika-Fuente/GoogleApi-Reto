# GoogleApi-Reto

## Descripción
Este proyecto consiste en una aplicación web desarrollada con ASP.NET Core 8.0 que integra con las APIs de Google Calendar y Gmail para gestionar eventos del calendario y enviar correos electrónicos.

El proyecto es el resultado de un pequeño reto realizado en la empresa Integra en colaboración con 2 compañeros más, donde pusimos en práctica nuestros conocimientos en desarrollo web y la integración de APIs de terceros.

## Funcionalidades principales

### Integración con Google Calendar
- **Visualización de eventos**: Muestra los eventos próximos del calendario del usuario
- **Creación de eventos**: Permite añadir nuevos eventos al calendario con título, descripción, fecha/hora de inicio y finalización
- **Edición de eventos**: Permite modificar los detalles de eventos existentes
- **Recordatorios**: Configuración automática de recordatorios para los eventos

### Integración con Gmail
- **Notificaciones por correo**: Envía correos electrónicos automáticos al crear eventos
- **Personalización**: Los correos incluyen detalles del evento creado

### Gestión de autenticación
- **Inicio de sesión con Google**: Autenticación OAuth 2.0 con Google
- **Gestión de sesiones**: Almacenamiento seguro de tokens y credenciales
- **Cambio de cuenta**: Opción para cambiar entre diferentes cuentas de Google

## Tecnologías utilizadas
- **Backend**: ASP.NET Core 8.0, C#
- **Arquitectura**: Patrón MVC (Model-View-Controller)
- **APIs**: Google Calendar API, Gmail API
- **Frontend**: HTML, CSS, JavaScript, Bootstrap
- **Autenticación**: OAuth 2.0 con Google

## Cómo empezar
1. Clona este repositorio
2. Configura tus credenciales de Google en el archivo `credentials.json`
3. Ejecuta la aplicación usando Visual Studio o dotnet CLI
4. Inicia sesión con tu cuenta de Google para comenzar a usar la aplicación
