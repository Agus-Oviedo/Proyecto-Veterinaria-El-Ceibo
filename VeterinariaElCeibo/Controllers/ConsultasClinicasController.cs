using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using VeterinariaElCeibo.Data;
using VeterinariaElCeibo.Models;

namespace VeterinariaElCeibo.Controllers
{
    // 👉 SOLO Administrador y Veterinario pueden usar historia clínica
    [Authorize(Roles = "Administrador,Veterinario")]
    public class ConsultasClinicasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConsultasClinicasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================== HISTORIA CLÍNICA DE UNA MASCOTA ==================
        public async Task<IActionResult> Index(int mascotaId)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == mascotaId);

            if (mascota == null)
                return NotFound();

            var consultas = await _context.ConsultasClinicas
                .Where(c => c.MascotaId == mascotaId)
                .Include(c => c.Veterinario)
                .OrderByDescending(c => c.FechaConsulta)
                .ToListAsync();

            ViewBag.Mascota = mascota;
            return View(consultas);
        }

        // ================== NUEVA CONSULTA (GET) ==================
        public async Task<IActionResult> Create(int mascotaId, int? turnoId)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == mascotaId);

            if (mascota == null)
                return NotFound();

            var consulta = new ConsultaClinica
            {
                MascotaId = mascotaId,
                FechaConsulta = DateTime.Today
            };

            if (turnoId.HasValue)
            {
                var turno = await _context.Turnos.FindAsync(turnoId.Value);
                if (turno != null)
                {
                    consulta.TurnoId = turnoId.Value;
                    consulta.Motivo = turno.Motivo;
                    consulta.FechaConsulta = turno.FechaHora.Date;
                }
            }

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag(consulta.VeterinarioId);

            return View(consulta);
        }

        // ================== NUEVA CONSULTA (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsultaClinica consulta)
        {
            if (consulta.MascotaId == 0)
            {
                ModelState.AddModelError(string.Empty, "Falta la mascota de la consulta.");
            }

            // veterinario elegido desde el combo
            if (string.IsNullOrEmpty(consulta.VeterinarioId))
            {
                ModelState.AddModelError(nameof(ConsultaClinica.VeterinarioId), "Debe seleccionar un veterinario.");
            }

            if (ModelState.IsValid)
            {
                consulta.FechaConsulta = consulta.FechaConsulta.Date;

                _context.Add(consulta);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Consulta registrada correctamente.";
                return RedirectToAction(nameof(Index), new { mascotaId = consulta.MascotaId });
            }

            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == consulta.MascotaId);

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag(consulta.VeterinarioId);

            return View(consulta);
        }

        // ================== EDITAR CONSULTA (GET) ==================
        public async Task<IActionResult> Edit(int id)
        {
            var consulta = await _context.ConsultasClinicas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.Cliente)
                .Include(c => c.Veterinario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
                return NotFound();

            ViewBag.Mascota = consulta.Mascota;
            await CargarVeterinariosEnViewBag(consulta.VeterinarioId);

            // devolvemos una copia sin navegaciones para evitar problemas de tracking
            return View(COPIAR_ConsultaSinNavegacion(consulta));
        }

        // ================== EDITAR CONSULTA (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConsultaClinica consulta)
        {
            if (id != consulta.Id)
                return NotFound();

            if (consulta.MascotaId == 0)
            {
                ModelState.AddModelError(string.Empty, "Falta la mascota de la consulta.");
            }

            if (string.IsNullOrEmpty(consulta.VeterinarioId))
            {
                ModelState.AddModelError(nameof(ConsultaClinica.VeterinarioId), "Debe seleccionar un veterinario.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    consulta.FechaConsulta = consulta.FechaConsulta.Date;

                    _context.Update(consulta);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Consulta actualizada correctamente.";
                    return RedirectToAction(nameof(Index), new { mascotaId = consulta.MascotaId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultaExists(consulta.Id))
                        return NotFound();

                    TempData["ErrorMessage"] = "Ocurrió un error al actualizar la consulta.";
                    throw;
                }
            }

            var mascota = await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == consulta.MascotaId);

            ViewBag.Mascota = mascota;
            await CargarVeterinariosEnViewBag(consulta.VeterinarioId);

            return View(consulta);
        }

        // ================== DETALLE DE CONSULTA ==================
        public async Task<IActionResult> Details(int id)
        {
            var consulta = await _context.ConsultasClinicas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.Cliente)
                .Include(c => c.Veterinario)
                .Include(c => c.Turno)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
                return NotFound();

            var tieneInternacion = await _context.Internaciones
                .AnyAsync(i => i.ConsultaIngresoId == id);

            ViewBag.TieneInternacion = tieneInternacion;

            return View(consulta);
        }

        // ================== ELIMINAR CONSULTA ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var consulta = await _context.ConsultasClinicas
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
            {
                TempData["ErrorMessage"] = "No se encontró la consulta clínica a eliminar.";
                return RedirectToAction("Index", "Mascotas");
            }

            var mascotaId = consulta.MascotaId;

            // No dejar borrar si tiene internación
            var internacion = await _context.Internaciones
                .FirstOrDefaultAsync(i => i.ConsultaIngresoId == id);

            if (internacion != null)
            {
                TempData["ErrorMessage"] =
                    "No se puede eliminar la consulta porque tiene una internación asociada. " +
                    "Revisá primero la hoja de internación.";

                return RedirectToAction(nameof(Index), new { mascotaId });
            }

            _context.ConsultasClinicas.Remove(consulta);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "Consulta clínica eliminada correctamente.";

            return RedirectToAction(nameof(Index), new { mascotaId });
        }

        // ================== MÉTODOS PRIVADOS ==================
        private bool ConsultaExists(int id)
        {
            return _context.ConsultasClinicas.Any(e => e.Id == id);
        }

        private async Task CargarVeterinariosEnViewBag(string? veterinarioSeleccionadoId = null)
        {
            var vets = await _userManager.GetUsersInRoleAsync("Veterinario");

            ViewBag.Veterinarios = vets
                .Select(v => new SelectListItem
                {
                    Value = v.Id,
                    Text = $"{v.Nombre} {v.Apellido}",
                    Selected = (v.Id == veterinarioSeleccionadoId)
                })
                .ToList();
        }

        private ConsultaClinica COPIAR_ConsultaSinNavegacion(ConsultaClinica c)
        {
            return new ConsultaClinica
            {
                Id = c.Id,
                MascotaId = c.MascotaId,
                TurnoId = c.TurnoId,
                FechaConsulta = c.FechaConsulta,
                Motivo = c.Motivo,
                Anamnesis = c.Anamnesis,
                ExamenFisico = c.ExamenFisico,
                PesoKg = c.PesoKg,
                TemperaturaC = c.TemperaturaC,
                Diagnostico = c.Diagnostico,
                Tratamiento = c.Tratamiento,
                Indicaciones = c.Indicaciones,
                ProximoControl = c.ProximoControl,
                VeterinarioId = c.VeterinarioId
            };
        }
    }
}
