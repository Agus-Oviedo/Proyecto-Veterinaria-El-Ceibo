using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaElCeibo.Models
{
    public class Vacunacion
    {
        public int Id { get; set; }

        // ---------- RELACIÓN CON MASCOTA -------------------
        [Required]
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }

        // ---------- DATOS DE LA VACUNA ---------------------
        [Required(ErrorMessage = "La fecha de la vacuna es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de aplicación")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El nombre de la vacuna es obligatorio.")]
        [MaxLength(100)]
        [Display(Name = "Vacuna")]
        public string Vacuna { get; set; } = string.Empty;

        // ---------- VETERINARIO (USUARIO DEL SISTEMA) -------
        [Display(Name = "Veterinario")]
        public string? VeterinarioId { get; set; }   // FK a ApplicationUser

        public ApplicationUser? Veterinario { get; set; }
    }
}
