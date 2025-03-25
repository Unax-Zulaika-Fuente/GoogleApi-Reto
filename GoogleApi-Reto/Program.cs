using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar Google Calendar API
builder.Services.AddSingleton<CalendarService>(provider =>
{
    // Cargar las credenciales de la API de Google desde el archivo credentials.json
    var clientSecrets = GoogleClientSecrets.FromStream(new FileStream("credentials.json", FileMode.Open, FileAccess.Read)).Secrets;

    // Crear el flujo de autorización
    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
    {
        ClientSecrets = clientSecrets,
        Scopes = new[] { CalendarService.Scope.Calendar },
    });

    // Obtener las credenciales (aquí asumimos que el usuario ya se ha autenticado previamente)
    var credential = flow.LoadTokenAsync("user", CancellationToken.None).Result;

    // Crear y devolver el servicio de CalendarService con las credenciales obtenidas
    return new CalendarService(new BaseClientService.Initializer()
    {
        HttpClientInitializer = (Google.Apis.Http.IConfigurableHttpClientInitializer)credential,
        ApplicationName = "GoogleApi-Reto",
    });
});

builder.Services.AddHttpContextAccessor();
// Si en un tiempo (30 minutos) no se hace ninguna petición, la sesión se invalida
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GoogleCalendar}/{action=Index}/{id?}");

app.UseSession();

app.Run();
