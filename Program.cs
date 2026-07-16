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

app.UseStaticFiles(); // Cambiado

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();