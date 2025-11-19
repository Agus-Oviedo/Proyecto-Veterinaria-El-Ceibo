using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaElCeibo.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [MaxLength(20)]
        [Display(Name = "DNI")]
        public string Dni { get; set; }   // obligatorio

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(30)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }  // obligatorio

        [MaxLength(100)]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingresá un correo válido.")]
        public string? Email { get; set; }    // OPCIONAL

        [MaxLength(200)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; } // OPCIONAL

        [MaxLength(100)]
        public string? Localidad { get; set; } // OPCIONAL

        [Display(Name = "Fecha de alta")]
        [DataType(DataType.Date)]
        public DateTime FechaAlta { get; set; } = DateTime.Today;

        [MaxLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; } // OPCIONAL

        // -------- NUEVO: nombre completo para mostrar en combos, listas, etc. --------
        [NotMapped]
        [Display(Name = "Cliente")]
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
