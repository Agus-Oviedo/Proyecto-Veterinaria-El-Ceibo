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
    // 👉 SOLO Administrador y Veterinario
    [Authorize(Roles = "Administrador,Veterinario")]
    public class InternacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InternacionesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================== LISTA DE INTERNACIONES ACTIVAS ==================
        // GET: Internaciones/Activas
        public async Task<IActionResult> Activas()
        {
            var internaciones = await _context.Internaciones
                .Where(i => i.Estado == "Activa" && i.FechaAlta == null)
                .Include(i => i.Mascota)
                    .ThenInclude(m => m.Cliente)
                .Include(i => i.ConsultaIngreso)
                .OrderBy(i => i.FechaIngreso)
                .ToListAsync();

            return View(internaciones);
        }

        // ================== INICIAR INTERNACIÓN (CREA EPISODIO) ==================
        // POST: Internaciones/Iniciar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(int consultaId)
        {
            var consulta = await _context.ConsultasClinicas
                .Include(c => c.Mascota)
                .FirstOrDefaultAsync(c => c.Id == consultaId);

            if (consulta == null)
            {
                TempData["ErrorMessage"] = "No se encontró la consulta seleccionada.";
                return RedirectToAction("Index", "Mascotas");
            }

            // Si ya existe internación para esta consulta, no creamos otra
            var existente = await _context.Internaciones
                .FirstOrDefaultAsync(i => i.ConsultaIngresoId == consultaId);

            if (existente != null)
            {
                TempData["ErrorMessage"] = "La consulta ya tiene una internación iniciada.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            var internacion = new Internacion
            {
                MascotaId = consulta.MascotaId,
                ConsultaIngresoId = consulta.Id,
                FechaIngreso = DateTime.Now,
                MotivoIngreso = !string.IsNullOrWhiteSpace(consulta.Diagnostico)
                    ? consulta.Diagnostico
                    : consulta.Motivo,
                Estado = "Activa"
            };

            _context.Internaciones.Add(internacion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Internación iniciada correctamente.";

            return RedirectToAction(nameof(Hoja), new { consultaId });
        }

        // ================== HOJA DE INTERNACIÓN (SOLO VER / EDITAR) ==================
        // NO crea internación. Si no existe, vuelve a la consulta con error.
        public async Task<IActionResult> Hoja(int consultaId)
        {
            var internacion = await _context.Internaciones
                .Include(i => i.Mascota)
                    .ThenInclude(m => m.Cliente)
                .Include(i => i.ConsultaIngreso)
                .Include(i => i.Registros)
                    .ThenInclude(r => r.Veterinario)
                .FirstOrDefaultAsync(i => i.ConsultaIngresoId == consultaId);

            if (internacion == null)
            {
                TempData["ErrorMessage"] =
                    "Esta consulta no tiene una internación iniciada.";
                return RedirectToAction("Details", "ConsultasClinicas", new { id = consultaId });
            }

            await CargarVeterinariosEnViewBag(); // 👉 combo para nuevo registro

            return View(internacion);
        }

        // ================== AGREGAR REGISTRO A LA HOJA ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarRegistro(
            int internacionId,
            int consultaId,
            string descripcion,
            decimal? pesoKg,
            decimal? temperaturaC,
            string veterinarioId)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                TempData["ErrorMessage"] = "La descripción es obligatoria.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            if (string.IsNullOrEmpty(veterinarioId))
            {
                TempData["ErrorMessage"] = "Debe seleccionar un veterinario para el registro.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            var internacion = await _context.Internaciones
                .FirstOrDefaultAsync(i => i.Id == internacionId);

            if (internacion == null)
            {
                TempData["ErrorMessage"] = "No se encontró la internación.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            // Si ya está de alta, no dejar agregar
            if (internacion.Estado == "Alta" && internacion.FechaAlta.HasValue)
            {
                TempData["ErrorMessage"] = "La internación ya está dada de alta. No se pueden agregar más registros.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            var registro = new RegistroInternacion
            {
                InternacionId = internacionId,
                FechaHora = DateTime.Now,
                Descripcion = descripcion,
                PesoKg = pesoKg,
                TemperaturaC = temperaturaC,
                VeterinarioId = veterinarioId
            };

            _context.RegistrosInternacion.Add(registro);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registro de internación agregado correctamente.";

            return RedirectToAction(nameof(Hoja), new { consultaId });
        }

        // ================== DAR ALTA A UNA INTERNACIÓN ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DarAlta(int id, int consultaId)
        {
            var internacion = await _context.Internaciones
                .FirstOrDefaultAsync(i => i.Id == id);

            if (internacion == null)
            {
                TempData["ErrorMessage"] = "No se encontró la internación.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            if (internacion.Estado == "Alta" && internacion.FechaAlta != null)
            {
                TempData["ErrorMessage"] = "La internación ya está dada de alta.";
                return RedirectToAction(nameof(Hoja), new { consultaId });
            }

            internacion.Estado = "Alta";
            internacion.FechaAlta = DateTime.Now;

            _context.Update(internacion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Internación dada de alta correctamente.";

            return RedirectToAction(nameof(Hoja), new { consultaId });
        }

        // ================== MÉTODOS PRIVADOS ==================
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
