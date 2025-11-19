using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VeterinariaElCeibo.Models
{
    public class RegistroInternacion
    {
        public int Id { get; set; }

        [Required]
        public int InternacionId { get; set; }

        [ValidateNever]
        public Internacion? Internacion { get; set; }

        [Required]
        [Display(Name = "Fecha y hora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        // Opcional: quién cargó el registro
        public string? VeterinarioId { get; set; }

        [ValidateNever]
        public ApplicationUser? Veterinario { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Peso (kg)")]
        public decimal? PesoKg { get; set; }

        [Column(TypeName = "decimal(4,1)")]
        [Display(Name = "Temperatura (°C)")]
        public decimal? TemperaturaC { get; set; }
    }
}
