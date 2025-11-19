using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VeterinariaElCeibo.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        // ---------- Relación con Cliente ----------

        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        // Navegación: no se valida en los formularios (solo se usa para mostrar)
        [ValidateNever]
        public Cliente? Cliente { get; set; }

        // ---------- Datos de la mascota ----------

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [MaxLength(50)]
        [Display(Name = "Nombre de la mascota")]
        public string NombreMascota { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria.")]
        [MaxLength(30)]
        public string Especie { get; set; } = string.Empty;

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

        // Edad calculada en base a la fecha de nacimiento (no se guarda en BD)
        [NotMapped]
        [Display(Name = "Edad (calculada)")]
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
