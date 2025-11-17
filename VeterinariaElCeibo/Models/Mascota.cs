using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaElCeibo.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        // 👉 Hacerla nullable para que no sea requerida por el validador
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [MaxLength(50)]
        [Display(Name = "Nombre de la mascota")]
        public string NombreMascota { get; set; }

        [Required(ErrorMessage = "La especie es obligatoria.")]
        [MaxLength(30)]
        public string Especie { get; set; }

        [MaxLength(50)]
        public string? Raza { get; set; }

        [MaxLength(30)]
        public string? Chip { get; set; }

        [Display(Name = "Castrado/a")]
        public bool Castrado { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Edad (años)")]
        public int? Edad { get; set; }

        [NotMapped]
        public int? EdadCalculada
        {
            get
            {
                if (FechaNacimiento.HasValue)
                {
                    var hoy = DateTime.Today;
                    var edad = hoy.Year - FechaNacimiento.Value.Year;
                    if (FechaNacimiento.Value.Date > hoy.AddYears(-edad))
                        edad--;
                    return edad;
                }
                return Edad;
            }
        }

        [Display(Name = "Fecha de alta")]
        [DataType(DataType.Date)]
        public DateTime FechaAlta { get; set; } = DateTime.Today;

        [Display(Name = "Fecha de baja")]
        [DataType(DataType.Date)]
        public DateTime? FechaBaja { get; set; }

        [MaxLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }
}
