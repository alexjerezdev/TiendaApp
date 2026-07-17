using Microsoft.EntityFrameworkCore;
using TiendaApp.Data;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
// Servicios MVC
builder.Services.AddControllersWithViews();

// Configuración de PostgreSQL
var connectionString = "Host=localhost;Port=5432;Database=TiendaDb;Username=postgres;Password=alex20-+";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

// Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// RECOMENDACIÓN SOLID:
// Este middleware cumple con el Principio de Responsabilidad Única (SRP)
// porque solo se encarga de medir el rendimiento de cada petición.

app.Use(async (context, next) =>
{
    var timer = System.Diagnostics.Stopwatch.StartNew();

    // Agregamos un encabezado personalizado
    context.Response.Headers.Append("X-Server-Performance", "Tracking");

    await next();

    timer.Stop();

    Console.WriteLine(
        $"[MONITOR] {context.Request.Method} {context.Request.Path} - {timer.ElapsedMilliseconds} ms");
});

app.Map("/sitemap.xml", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/xml";

        var sitemapContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">

    <url>
        <loc>https://localhost:5244/</loc>
        <priority>1.0</priority>
    </url>

    <url>
        <loc>https://localhost:5244/Productos</loc>
        <priority>0.8</priority>
    </url>

    <url>
        <loc>https://localhost:5244/promociones-del-mes/barrio-norte</loc>
        <priority>0.9</priority>
    </url>

</urlset>";

        await context.Response.WriteAsync(sitemapContent);
    });
});

app.UseStaticFiles(); // Cambiado

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();