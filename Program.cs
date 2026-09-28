using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Habilita la arquitectura MVC
builder.Services.AddControllersWithViews();

// Configuración de Base de Datos con Resiliencia ante fallos transitorios
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

// Configuración de Autenticación por Cookies (Nativo de ASP.NET Core)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

var app = builder.Build();

// Creación automática de base de datos, aplicación de migraciones y siembra inicial (Seed Data)
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

// Middleware de Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// Por defecto abrimos la pantalla de Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
