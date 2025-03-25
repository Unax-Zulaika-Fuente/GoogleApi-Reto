

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleApi_Reto.DataStore;
using GoogleApi_Reto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace GoogleApi_Reto.Controllers
{
    public class GoogleCalendarController : Controller
    {
        private static string[] Scopes = { CalendarService.Scope.Calendar };

        
        public GoogleCalendarController()
        {
        }

        private async Task<UserCredential> GetGoogleCredential(HttpContext httpContext, bool isIndex)
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = "971462718389-5iplr0gv5casfdbctn8otsrhses6ffqa.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            // Crear el almacén de datos basado en la sesión del usuario
            var sessionDataStore = new SessionDataStore(httpContext);

            // Crear el flujo de autorización utilizando el almacén de sesión
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = Scopes,
                DataStore = sessionDataStore
            });

            // Usar el identificador de sesión como clave única para el usuario
            var userKey = httpContext.Session.Id;

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Scopes,
                userKey,
                CancellationToken.None,
                sessionDataStore
            );

            return credential;
        }


        //private static async Task<UserCredential> GetGoogleCredential()
        //{
        //    var clientSecrets = new ClientSecrets
        //    {
        //        ClientId = "971462718389-5iplr0gv5casfdbctn8otsrhses6ffqa.apps.googleusercontent.com",
        //        ClientSecret = "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"
        //    };

        //    // Crear el flujo de autorización
        //    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = clientSecrets,
        //        Scopes = Scopes,
        //        DataStore = new FileDataStore("Google.Apis.Auth")
        //    });

        //    // Cargar el token o solicitar autorización
        //    var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        clientSecrets,
        //        Scopes,
        //        "user", // Nombre de la cuenta
        //        CancellationToken.None,
        //        new FileDataStore("Google.Apis.Auth", true)
        //    );

        //    return credential;
        //}

        private static CalendarService GetCalendarService(UserCredential credential)
        {
            // Crear el servicio de Google Calendar
            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleApi-Reto"
            });
        }

        public async Task<IActionResult> Index()
        {
            bool isIndex = true;
            // Obtener las credenciales de Google
            var credential = await GetGoogleCredential(HttpContext, isIndex);

            // Crear el servicio de Google Calendar
            var service = GetCalendarService(credential);

            // Obtener los eventos del calendario
            var eventsRequest = service.Events.List("primary");
            eventsRequest.TimeMin = DateTime.Today;  // Eventos desde hoy (medianoche)
            eventsRequest.SingleEvents = true;       // Expande eventos recurrentes
            eventsRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime; // Ordenar por hora de inicio

            var events = await eventsRequest.ExecuteAsync();

            // Ordena los eventos (hay que tener en cuenta que algunos eventos pueden tener DateTime o solo Date)
            var model = events.Items.OrderBy(e =>
                e.Start.DateTime != null ? e.Start.DateTime : DateTime.Parse(e.Start.Date)
            ).ToList();

            return View(model ?? new List<Event>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var newEvent = new GoogleCalendarEventInsert();
            return View(newEvent); ;
        }

        [HttpPost]
        public async Task<IActionResult> Create(GoogleCalendarEventInsert evt)
        {
            // Obtener las credenciales de Google
            var credential = await GetGoogleCredential(HttpContext, false);

            // Crear el servicio de Google Calendar
            var service = GetCalendarService(credential);

            Event newEvent = new Event();

            if (ModelState.IsValid)
            {
                newEvent = new Event
                {
                    Summary = evt.Summary,
                    Description = evt.Description,
                    Start = new EventDateTime { DateTimeDateTimeOffset = evt.Start },                 
                    End = new EventDateTime { DateTimeDateTimeOffset = evt.End }
                };

                var eventRequest = service.Events.Insert(newEvent, "primary");
                await eventRequest.ExecuteAsync();
                return RedirectToAction("Index");
            }
            return View(newEvent); // Si el modelo no es válido, volver a la vista con el evento
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string eventId)
        {
            // Obtener las credenciales de Google
            var credential = await GetGoogleCredential(HttpContext, false);

            // Crear el servicio de Google Calendar
            var service = GetCalendarService(credential);

            var eventRequest = service.Events.Get("primary", eventId);
            var eventDetails = await eventRequest.ExecuteAsync();

            return View(eventDetails);
        }

        [HttpGet]
        public IActionResult ChangeAccount()
        {
            // Limpia la sesion para borrar el token de Google (y otros datos, si los hubiera)
            HttpContext.Session.Clear();
            // Redirige a Index para que se inicie el proceso de autenticacion nuevamente
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Event updatedEvent)
        {
            // Obtener las credenciales de Google
            var credential = await GetGoogleCredential(HttpContext, false);

            // Crear el servicio de Google Calendar
            var service = GetCalendarService(credential);

            if (ModelState.IsValid)
            {
                var eventRequest = service.Events.Update(updatedEvent, "primary", updatedEvent.Id);
                await eventRequest.ExecuteAsync();
                return RedirectToAction("Index");
            }
            return View(updatedEvent);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string eventId)
        {
            // Obtener las credenciales de Google
            var credential = await GetGoogleCredential(HttpContext, false);

            // Crear el servicio de Google Calendar
            var service = GetCalendarService(credential);

            await service.Events.Delete("primary", eventId).ExecuteAsync();
            return RedirectToAction("Index");
        }
    }
}
