using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VeterinariaElCeibo.Models
{
    public class Internacion
    {
        public int Id { get; set; }

        [Required]
        public int MascotaId { get; set; }

        [ValidateNever]
        public Mascota? Mascota { get; set; }

        // Consulta clínica de ingreso (opcional, pero la vamos a usar)
        public int? ConsultaIngresoId { get; set; }

        [ValidateNever]
        public ConsultaClinica? ConsultaIngreso { get; set; }

        [Required]
        [Display(Name = "Fecha de ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de alta")]
        public DateTime? FechaAlta { get; set; }

        [StringLength(200)]
        [Display(Name = "Motivo de ingreso")]
        public string? MotivoIngreso { get; set; }

        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activa"; // Activa / Alta

        [ValidateNever]
        public ICollection<RegistroInternacion> Registros { get; set; }
            = new List<RegistroInternacion>();
    }
}
