using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VeterinariaElCeibo.Models
{
    public class ConsultaClinica
    {
        public int Id { get; set; }

        // ---------- Relaciones principales ----------

        [Required]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

        [ValidateNever]           // <- NO validar navegación
        public Mascota? Mascota { get; set; }   // <- nullable

        // Turno del que viene la consulta (opcional)
        [Display(Name = "Turno asociado")]
        public int? TurnoId { get; set; }

        [ValidateNever]
        public Turno? Turno { get; set; }       // <- nullable

        // Veterinario (usuario Identity)
        [Display(Name = "Veterinario")]
        public string? VeterinarioId { get; set; }   // <- SIN [Required]

        [ValidateNever]
        public ApplicationUser? Veterinario { get; set; } // <- nullable

        // ---------- Datos de la consulta ----------

        [Required]
        [Display(Name = "Fecha de consulta")]
        [DataType(DataType.Date)]
        public DateTime FechaConsulta { get; set; } = DateTime.Today;

        [Required]
        [StringLength(200)]
        [Display(Name = "Motivo de consulta")]
        public string Motivo { get; set; }

        [Display(Name = "Relato del dueño / Anamnesis")]
        [DataType(DataType.MultilineText)]
        public string? Anamnesis { get; set; }

        [Display(Name = "Examen físico / Hallazgos")]
        [DataType(DataType.MultilineText)]
        public string? ExamenFisico { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Peso (kg)")]
        public decimal? PesoKg { get; set; }

        [Column(TypeName = "decimal(4,1)")]
        [Display(Name = "Temperatura (°C)")]
        public decimal? TemperaturaC { get; set; }

        [Display(Name = "Diagnóstico")]
        [DataType(DataType.MultilineText)]
        public string? Diagnostico { get; set; }

        [Display(Name = "Tratamiento / Medicación")]
        [DataType(DataType.MultilineText)]
        public string? Tratamiento { get; set; }

        [Display(Name = "Indicaciones al tutor")]
        [DataType(DataType.MultilineText)]
        public string? Indicaciones { get; set; }

        [Display(Name = "Próximo control")]
        [DataType(DataType.Date)]
        public DateTime? ProximoControl { get; set; }
    }
}
