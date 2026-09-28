using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Habilita la arquitectura MVC (¡Esta línea faltaba!)
builder.Services.AddControllersWithViews();

// Configuración de Base de Datos
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
