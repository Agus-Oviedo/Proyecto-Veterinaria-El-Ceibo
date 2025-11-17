using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaElCeibo.Data;
using VeterinariaElCeibo.Models;

namespace VeterinariaElCeibo.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Clientes
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .ToListAsync();

            return View(lista);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cliente creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al crear el cliente.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Si algo falla, volvemos a mostrar el formulario con los errores de validación
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cliente modificado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clientes.Any(e => e.Id == cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ocurrió un error de concurrencia al modificar el cliente.";
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al modificar el cliente.";
                }
            }

            // Si hay errores de validación o algo falló, volvemos a mostrar el form
            return View(cliente);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Delete/5
        // No usamos vista de confirmación porque la confirmación se hace con SweetAlert2 en el Index.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            // Redirigimos al Index; el borrado real se hace en DeleteConfirmed (POST)
            return RedirectToAction(nameof(Index));
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    TempData["ErrorMessage"] = "El cliente que intentás eliminar no existe.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                // Usamos DeleteMessage para que el layout muestre el toast rojo
                TempData["DeleteMessage"] = "Cliente eliminado correctamente.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el cliente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
