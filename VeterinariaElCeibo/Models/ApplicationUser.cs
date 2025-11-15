using Microsoft.AspNetCore.Identity;
using System;

namespace VeterinariaElCeibo.Models
{
    // Usuario de la aplicación (extiende el usuario base de Identity)
    public class ApplicationUser : IdentityUser
    {
        // NO los marco como [Required] todavía para no romper la migración
        // más adelante podemos validarlos desde los formularios.

        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? DNI { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
