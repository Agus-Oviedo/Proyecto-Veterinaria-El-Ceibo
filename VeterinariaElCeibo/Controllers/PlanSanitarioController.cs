using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using VeterinariaElCeibo.Data;
using VeterinariaElCeibo.Models;
using VeterinariaElCeibo.ViewModels;

namespace VeterinariaElCeibo.Controllers
{
    // 👉 Solo Administrador y Veterinario gestionan plan sanitario
    [Authorize(Roles = "Administrador,Veterinario")]
    public class PlanSanitarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlanSanitarioController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================== LIBRETA SANITARIA ==================
        // GET: PlanSanitario?mascotaId=5
        public async Task<IActionResult> Index(int mascotaId)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == mascotaId);

            if (mascota == null) return NotFound();

            var vacunas = await _context.Vacunaciones
                .Where(v => v.MascotaId == mascotaId)
                .Include(v => v.Veterinario)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            var desparas = await _context.Desparasitaciones
                .Where(d => d.MascotaId == mascotaId)
                .Include(d => d.Veterinario)
                .OrderByDescending(d => d.Fecha)
                .ToListAsync();

            var vm = new PlanSanitarioViewModel
            {
                Mascota = mascota,
                Vacunaciones = vacunas,
                Desparasitaciones = desparas
            };

            return View(vm);
        }

        // ================== CREATE VACUNA (GET) ==================
        public async Task<IActionResult> CreateVacuna(int mascotaId)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == mascotaId);

            if (mascota == null) return NotFound();

            await CargarVeterinariosEnViewBag();
            ViewBag.Mascota = mascota;

            var modelo = new Vacunacion
            {
                MascotaId = mascotaId,
                Fecha = DateTime.Today
            };

            return View(modelo);
        }

        // ================== CREATE VACUNA (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVacuna(Vacunacion modelo)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == modelo.MascotaId);

            if (mascota == null) return NotFound();

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "No se pudo registrar la vacuna. Revisá los datos ingresados.";
                return View(modelo);
            }

            try
            {
                _context.Vacunaciones.Add(modelo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vacuna registrada correctamente.";
                return RedirectToAction(nameof(Index), new { mascotaId = modelo.MascotaId });
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al guardar la vacuna.";
                return View(modelo);
            }
        }

        // ================== EDIT VACUNA (GET) ==================
        public async Task<IActionResult> EditVacuna(int id)
        {
            var vacuna = await _context.Vacunaciones
                .Include(v => v.Mascota)
                    .ThenInclude(m => m.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacuna == null) return NotFound();

            await CargarVeterinariosEnViewBag();
            ViewBag.Mascota = vacuna.Mascota;

            return View(vacuna);
        }

        // ================== EDIT VACUNA (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVacuna(int id, Vacunacion modelo)
        {
            if (id != modelo.Id) return NotFound();

            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == modelo.MascotaId);

            if (mascota == null) return NotFound();

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "No se pudo modificar la vacuna. Revisá los datos.";
                return View(modelo);
            }

            try
            {
                _context.Update(modelo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vacuna modificada correctamente.";
                return RedirectToAction(nameof(Index), new { mascotaId = modelo.MascotaId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Vacunaciones.AnyAsync(v => v.Id == modelo.Id))
                    return NotFound();

                TempData["ErrorMessage"] = "Error de concurrencia al modificar la vacuna.";
                return View(modelo);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al modificar la vacuna.";
                return View(modelo);
            }
        }

        // ================== DELETE VACUNA ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVacuna(int id)
        {
            var vacuna = await _context.Vacunaciones.FindAsync(id);
            if (vacuna == null)
            {
                TempData["ErrorMessage"] = "La vacuna que intentás eliminar no existe.";
                return RedirectToAction("Index", "Clientes");
            }

            var mascotaId = vacuna.MascotaId;

            _context.Vacunaciones.Remove(vacuna);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "Vacuna eliminada correctamente.";
            return RedirectToAction(nameof(Index), new { mascotaId });
        }

        // ================== CREATE DESPARASITACIÓN (GET) ==================
        public async Task<IActionResult> CreateDesparasitacion(int mascotaId)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == mascotaId);

            if (mascota == null) return NotFound();

            await CargarVeterinariosEnViewBag();
            ViewBag.Mascota = mascota;

            var modelo = new Desparasitacion
            {
                MascotaId = mascotaId,
                Fecha = DateTime.Today
            };

            return View(modelo);
        }

        // ================== CREATE DESPARASITACIÓN (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDesparasitacion(Desparasitacion modelo)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == modelo.MascotaId);

            if (mascota == null) return NotFound();

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "No se pudo registrar la desparasitación. Revisá los datos.";
                return View(modelo);
            }

            try
            {
                _context.Desparasitaciones.Add(modelo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Desparasitación registrada correctamente.";
                return RedirectToAction(nameof(Index), new { mascotaId = modelo.MascotaId });
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al guardar la desparasitación.";
                return View(modelo);
            }
        }

        // ================== EDIT DESPARASITACIÓN (GET) ==================
        public async Task<IActionResult> EditDesparasitacion(int id)
        {
            var despara = await _context.Desparasitaciones
                .Include(d => d.Mascota)
                    .ThenInclude(m => m.Cliente)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (despara == null) return NotFound();

            await CargarVeterinariosEnViewBag();
            ViewBag.Mascota = despara.Mascota;

            return View(despara);
        }

        // ================== EDIT DESPARASITACIÓN (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDesparasitacion(int id, Desparasitacion modelo)
        {
            if (id != modelo.Id) return NotFound();

            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == modelo.MascotaId);

            if (mascota == null) return NotFound();

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "No se pudo modificar la desparasitación. Revisá los datos.";
                return View(modelo);
            }

            try
            {
                _context.Update(modelo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Desparasitación modificada correctamente.";
                return RedirectToAction(nameof(Index), new { mascotaId = modelo.MascotaId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Desparasitaciones.AnyAsync(d => d.Id == modelo.Id))
                    return NotFound();

                TempData["ErrorMessage"] = "Error de concurrencia al modificar la desparasitación.";
                return View(modelo);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al modificar la desparasitación.";
                return View(modelo);
            }
        }

        // ================== DELETE DESPARASITACIÓN ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDesparasitacion(int id)
        {
            var despara = await _context.Desparasitaciones.FindAsync(id);
            if (despara == null)
            {
                TempData["ErrorMessage"] = "La desparasitación que intentás eliminar no existe.";
                return RedirectToAction("Index", "Clientes");
            }

            var mascotaId = despara.MascotaId;

            _context.Desparasitaciones.Remove(despara);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "Desparasitación eliminada correctamente.";
            return RedirectToAction(nameof(Index), new { mascotaId });
        }

        // ================== MÉTODO PRIVADO: LISTA VETERINARIOS ==================
        private async Task CargarVeterinariosEnViewBag()
        {
            var vets = await _userManager.GetUsersInRoleAsync("Veterinario");

            ViewBag.Veterinarios = vets
                .Select(v => new SelectListItem
                {
                    Value = v.Id,
                    Text = $"{v.Nombre} {v.Apellido}"
                })
                .ToList();
        }
    }
}
