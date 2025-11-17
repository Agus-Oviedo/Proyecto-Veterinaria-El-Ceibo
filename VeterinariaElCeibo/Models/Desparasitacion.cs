using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaElCeibo.Models
{
    public class Desparasitacion
    {
        public int Id { get; set; }

        // ---------- RELACIÓN CON MASCOTA -------------------
        [Required]
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }

        // ---------- DATOS DE LA DESPARASITACIÓN ------------
        [Required(ErrorMessage = "La fecha de la desparasitación es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de aplicación")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        [MaxLength(100)]
        [Display(Name = "Producto / droga")]
        public string Producto { get; set; } = string.Empty;

        // ---------- VETERINARIO (USUARIO DEL SISTEMA) -------
        [Display(Name = "Veterinario")]
        public string? VeterinarioId { get; set; }   // FK a ApplicationUser

        public ApplicationUser? Veterinario { get; set; }
    }
}
