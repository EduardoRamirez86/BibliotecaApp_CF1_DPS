using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Services;

var builder = WebApplication.CreateBuilder(args);
//EL ORDEN CORRECTO DE INVOCACION DE LOS MIDDLEWARE DE SEGURIDAD
//ES: 1) UseAuthentication() 2) UseAuthorization() OSEA VERIFICA LOS PERMISOS QUE TIENE
// Habilita la arquitectura MVC
builder.Services.AddControllersWithViews();
//CONTROLADORES MVC Y SUS VISTAS EN EL CONTENEDOR

// Configuración de Base de Datos
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionSQL"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Servicios de Lógica de Negocio y Seguridad
//SERVICIOS DE APLICACION
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

// Configuración de Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        //establece la ruta a la que se redirige automáticamente
        //un usuario no autenticado que intenta acceder a un controlador con
        //[Authorize]
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";//establece la vista
                                                        //a la que se redirige
                                                        //a un usuario cuando
                                                        //carece de los roles
                                                        //requeridos por
                                                        //[Authorize(Roles = ...)]
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddScoped<BibliotecaApp.Services.LibroService>();
builder.Services.AddScoped<BibliotecaApp.Services.PrestamoService>();

var app = builder.Build();

// Creación automática de base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<BibliotecaDbContext>();
        var authService = services.GetRequiredService<IAuthService>();
        await DbSeeder.InicializarAsync(context, authService);
        logger.LogInformation("Base de datos conectada, migrada y sembrada con éxito.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos.");
        throw;
    }
}

// Configurar el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//IMPORTANTE IMPORTANTE

// Middleware de Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// Codigo necesario para poder mapear las rutas de la web API
app.MapControllers();
//METODO PARA MAPEAR LAS RUTAS DE LOS CONTROLADORES WEB API

// Por defecto abrimos la pantalla de Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
