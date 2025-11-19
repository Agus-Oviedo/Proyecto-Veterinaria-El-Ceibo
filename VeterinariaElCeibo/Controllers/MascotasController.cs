using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using VeterinariaElCeibo.Data;
using VeterinariaElCeibo.Models;
using VeterinariaElCeibo.ViewModels;

namespace VeterinariaElCeibo.Controllers
{
    // 👉 Admin, Veterinario y Peluqueria pueden gestionar mascotas
    [Authorize(Roles = "Administrador,Veterinario,Peluqueria")]
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MascotasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mascotas?clienteId=5
        public async Task<IActionResult> Index(int clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound();

            var mascotas = await _context.Mascotas
                .Where(m => m.ClienteId == clienteId)
                .OrderBy(m => m.NombreMascota)
                .ToListAsync();

            var vm = new MascotasIndexViewModel
            {
                Cliente = cliente,
                Mascotas = mascotas
            };

            return View(vm);
        }

        // GET: Mascotas/Create?clienteId=5
        public async Task<IActionResult> Create(int clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound();

            ViewBag.Cliente = cliente;

            var mascota = new Mascota
            {
                ClienteId = clienteId,
                FechaAlta = DateTime.Today
            };

            return View(mascota);
        }

        // POST: Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mascota mascota)
        {
            // volvemos a cargar el cliente para la vista si algo falla
            var cliente = await _context.Clientes.FindAsync(mascota.ClienteId);
            ViewBag.Cliente = cliente;

            // La navegación Cliente no viene del form, la sacamos de la validación
            ModelState.Remove(nameof(Mascota.Cliente));

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Datos inválidos.";

                TempData["ErrorMessage"] = "No se pudo guardar la mascota: " + firstError;
                return View(mascota);
            }

            _context.Add(mascota);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mascota creada correctamente.";
            return RedirectToAction(nameof(Index), new { clienteId = mascota.ClienteId });
        }

        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mascota == null) return NotFound();

            return View(mascota);
        }

        // POST: Mascotas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mascota mascota)
        {
            if (id != mascota.Id) return NotFound();

            ModelState.Remove(nameof(Mascota.Cliente));

            if (!ModelState.IsValid)
            {
                return View(mascota);
            }

            try
            {
                _context.Update(mascota);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Mascota modificada correctamente.";
                return RedirectToAction(nameof(Index), new { clienteId = mascota.ClienteId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Mascotas.AnyAsync(m => m.Id == mascota.Id))
                    return NotFound();

                TempData["ErrorMessage"] = "Ocurrió un error de concurrencia al modificar la mascota.";
                return View(mascota);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al modificar la mascota.";
                return View(mascota);
            }
        }

        // POST: Mascotas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
            {
                TempData["ErrorMessage"] = "La mascota que intentás eliminar no existe.";
                return RedirectToAction("Index", "Clientes");
            }

            var clienteId = mascota.ClienteId;

            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "Mascota eliminada correctamente.";
            return RedirectToAction(nameof(Index), new { clienteId });
        }
    }
}
