using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApp.Data;
using TiendaApp.Models;
namespace TiendaApp.Controllers;
public class ProductosController : Controller
{
private readonly ApplicationDbContext _context;
// INYECCIÓN DE DEPENDENCIAS (DIP de SOLID): No instanciamos el contexto, lo recibimos.
public ProductosController(ApplicationDbContext context) => _context = context;
// READ: Obtener lista (Usamos Async para escalabilidad)
public async Task<IActionResult> Index()
{
// LINQ: AsNoTracking mejora rendimiento en solo lectura (Código Limpio)
var productos = await _context.Productos.AsNoTracking().ToListAsync();
return View(productos);
}
// CREATE: Vista
public IActionResult Create() => View();
// CREATE: Proceso POST
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Producto producto)
{
if (ModelState.IsValid)
{
_context.Add(producto);
await _context.SaveChangesAsync();
return RedirectToAction(nameof(Index));
}
return View(producto);
}
// GET: Productos/Edit/5
public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
        return NotFound();

    var producto = await _context.Productos.FindAsync(id);

    if (producto == null)
        return NotFound();

    return View(producto);
}

// POST: Productos/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Producto producto)
{
    if (id != producto.Id)
        return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(producto);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Productos.Any(e => e.Id == producto.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    return View(producto);
}

// ===================== DELETE =====================

// GET: Productos/Delete/5
public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
        return NotFound();

    var producto = await _context.Productos
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id);

    if (producto == null)
        return NotFound();

    return View(producto);
}

// POST: Productos/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var producto = await _context.Productos.FindAsync(id);

    if (producto != null)
    {
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
    }

    return RedirectToAction(nameof(Index));
}
}
