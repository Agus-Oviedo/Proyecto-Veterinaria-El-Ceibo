using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VeterinariaElCeibo.Models;

namespace VeterinariaElCeibo.Controllers
{
    // 👉 SOLO usuarios con rol "Administrador" pueden entrar acá
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ================== LISTA DE USUARIOS ==================
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            var modelo = new List<UsuarioRolesViewModel>();

            foreach (var u in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(u);

                modelo.Add(new UsuarioRolesViewModel
                {
                    UserId = u.Id,
                    Email = u.Email,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Roles = roles
                });
            }

            return View(modelo);
        }

        // ================== EDITAR ROLES DE UN USUARIO (GET) ==================
        public async Task<IActionResult> EditarRoles(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var todosLosRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var rolesUsuario = await _userManager.GetRolesAsync(usuario);

            var modelo = new EditarRolesUsuarioViewModel
            {
                UserId = usuario.Id,
                Email = usuario.Email,
                Roles = todosLosRoles.Select(r => new RolSeleccionadoViewModel
                {
                    Nombre = r,
                    Seleccionado = rolesUsuario.Contains(r)
                }).ToList()
            };

            return View(modelo);
        }

        // ================== EDITAR ROLES DE UN USUARIO (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarRoles(EditarRolesUsuarioViewModel modelo)
        {
            var usuario = await _userManager.FindByIdAsync(modelo.UserId);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            var resultadoRemove = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);

            if (!resultadoRemove.Succeeded)
            {
                TempData["ErrorMessage"] = "No se pudieron quitar los roles actuales.";
                return RedirectToAction(nameof(Index));
            }

            var rolesSeleccionados = modelo.Roles
                .Where(r => r.Seleccionado)
                .Select(r => r.Nombre)
                .ToList();

            if (rolesSeleccionados.Any())
            {
                var resultadoAdd = await _userManager.AddToRolesAsync(usuario, rolesSeleccionados);
                if (!resultadoAdd.Succeeded)
                {
                    TempData["ErrorMessage"] = "No se pudieron asignar los nuevos roles.";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["SuccessMessage"] = "Roles del usuario actualizados correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
