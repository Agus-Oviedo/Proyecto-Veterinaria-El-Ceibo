using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using VeterinariaElCeibo.Data;
using VeterinariaElCeibo.Models;

namespace VeterinariaElCeibo.Controllers
{
    // 👉 Admin, Veterinario y Peluqueria pueden gestionar turnos
    [Authorize(Roles = "Administrador,Veterinario,Peluqueria")]
    public class TurnosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TurnosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== INDEX (turnos del MES) =====================
        public async Task<IActionResult> Index(DateTime? fecha)
        {
            var fechaConsulta = fecha?.Date ?? DateTime.Today;

            var primerDiaMes = new DateTime(fechaConsulta.Year, fechaConsulta.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var turnos = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Cliente)
                .Where(t => t.FechaHora.Date >= primerDiaMes && t.FechaHora.Date <= ultimoDiaMes)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();

            ViewBag.FechaConsulta = fechaConsulta;
            ViewBag.PrimerDiaMes = primerDiaMes;

            return View(turnos);
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var turno = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.TurnoId == id);

            if (turno == null)
                return NotFound();

            return View(turno);
        }

        // ===================== CREATE (GET) =====================
        public IActionResult Create(int? mascotaId)
        {
            CargarComboClientes(null);

            ViewBag.MascotaId = new SelectList(new List<SelectListItem>(), "Value", "Text");

            return View();
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Turno turno, int? ClienteId)
        {
            if (!ClienteId.HasValue || ClienteId.Value == 0)
            {
                ModelState.AddModelError("ClienteId", "Debe seleccionar un cliente.");
            }

            if (turno.MascotaId == 0)
            {
                ModelState.AddModelError("MascotaId", "Debe seleccionar una mascota.");
            }

            if (ModelState.IsValid)
            {
                turno.FechaCreacion = DateTime.Now;
                _context.Add(turno);

                try
                {
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Turno creado correctamente.";
                    return RedirectToAction(nameof(Index), new { fecha = turno.FechaHora.Date });
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Error al guardar el turno.");
                    TempData["ErrorMessage"] = "Ocurrió un error al guardar el turno.";
                }
            }

            var errores = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            ViewBag.Errores = errores;

            int? clienteIdSeleccionado = ClienteId;
            if ((!clienteIdSeleccionado.HasValue || clienteIdSeleccionado.Value == 0) && turno.MascotaId != 0)
            {
                clienteIdSeleccionado = await _context.Mascotas
                    .Where(m => m.Id == turno.MascotaId)
                    .Select(m => (int?)m.ClienteId)
                    .FirstOrDefaultAsync();
            }

            CargarComboClientes(clienteIdSeleccionado);
            CargarComboMascotas(turno.MascotaId, clienteIdSeleccionado);

            return View(turno);
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var turno = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Cliente)
                .FirstOrDefaultAsync(t => t.TurnoId == id);

            if (turno == null)
                return NotFound();

            CargarComboMascotas(turno.MascotaId, turno.Mascota?.ClienteId);

            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Turno turno)
        {
            if (id != turno.TurnoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turno);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Turno actualizado correctamente.";
                    return RedirectToAction(nameof(Index), new { fecha = turno.FechaHora.Date });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoExists(turno.TurnoId))
                        return NotFound();

                    TempData["ErrorMessage"] = "Error de concurrencia al actualizar el turno.";
                    throw;
                }
            }

            CargarComboMascotas(turno.MascotaId, null);
            return View(turno);
        }

        // ===================== DELETE =====================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var turno = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.TurnoId == id);

            if (turno == null)
                return NotFound();

            return View(turno);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno != null)
            {
                _context.Turnos.Remove(turno);
                await _context.SaveChangesAsync();

                TempData["DeleteMessage"] = "Turno eliminado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se encontró el turno a eliminar.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== CAMBIAR ESTADO (SP) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var estadosValidos = new[] { "Pendiente", "Confirmado", "Atendido", "Cancelado" };
            if (!estadosValidos.Contains(nuevoEstado))
            {
                TempData["ErrorMessage"] = "Estado inválido.";
                return BadRequest("Estado inválido.");
            }

            var paramId = new SqlParameter("@TurnoId", id);
            var paramEstado = new SqlParameter("@NuevoEstado", nuevoEstado);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_Turnos_CambiarEstado @TurnoId, @NuevoEstado",
                paramId, paramEstado);

            TempData["SuccessMessage"] = "Estado del turno actualizado.";

            var turno = await _context.Turnos.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TurnoId == id);
            if (turno != null)
            {
                return RedirectToAction(nameof(Index), new { fecha = turno.FechaHora.Date });
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== ACCIONES JSON (AJAX) =====================

        [HttpGet]
        public async Task<IActionResult> ObtenerMascotasPorCliente(int clienteId)
        {
            var mascotas = await _context.Mascotas
                .Where(m => m.ClienteId == clienteId)
                .OrderBy(m => m.NombreMascota)
                .Select(m => new
                {
                    m.Id,
                    m.NombreMascota
                })
                .ToListAsync();

            return Json(mascotas);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTurnosPorFecha(DateTime fecha)
        {
            var fechaSolo = fecha.Date;

            var turnos = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Cliente)
                .Where(t => t.FechaHora.Date == fechaSolo)
                .OrderBy(t => t.FechaHora)
                .Select(t => new
                {
                    Hora = t.FechaHora.ToString("HH:mm"),
                    Mascota = t.Mascota.NombreMascota,
                    Cliente = t.Mascota.Cliente.NombreCompleto,
                    TipoTurno = t.TipoTurno,
                    Estado = t.EstadoTurno
                })
                .ToListAsync();

            return Json(turnos);
        }

        // ===================== MÉTODOS PRIVADOS =====================

        private void CargarComboClientes(int? clienteIdSeleccionado = null)
        {
            var clientes = _context.Clientes
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .Select(c => new
                {
                    c.Id,
                    c.NombreCompleto
                })
                .ToList();

            ViewBag.ClienteId = new SelectList(
                clientes,
                "Id",
                "NombreCompleto",
                clienteIdSeleccionado
            );
        }

        private void CargarComboMascotas(int? mascotaIdSeleccionada = null, int? clienteId = null)
        {
            var query = _context.Mascotas
                .Include(m => m.Cliente)
                .AsQueryable();

            if (clienteId.HasValue)
            {
                query = query.Where(m => m.ClienteId == clienteId.Value);
            }

            var mascotas = query
                .OrderBy(m => m.NombreMascota)
                .Select(m => new
                {
                    m.Id,
                    NombreCompleto = m.NombreMascota +
                                     (m.Cliente != null ? " (" + m.Cliente.NombreCompleto + ")" : "")
                })
                .ToList();

            ViewBag.MascotaId = new SelectList(
                mascotas,
                "Id",
                "NombreCompleto",
                mascotaIdSeleccionada
            );
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.TurnoId == id);
        }
    }
}
