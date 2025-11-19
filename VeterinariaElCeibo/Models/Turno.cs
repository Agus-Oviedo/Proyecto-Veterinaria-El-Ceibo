using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VeterinariaElCeibo.Models
{
    /// <summary>
    /// Representa un turno de atención para una mascota.
    /// NO es la historia clínica, solo la cita en la agenda.
    /// </summary>
    public class Turno
    {
        [Key]
        public int TurnoId { get; set; }

        // ---------- Relación con Mascota ----------

        /// <summary>
        /// Clave foránea a la mascota que viene al turno.
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar una mascota.")]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

        /// <summary>
        /// Propiedad de navegación a Mascota.
        /// No se valida en los formularios, solo se usa para mostrar datos relacionados.
        /// </summary>
        [ValidateNever]
        public Mascota? Mascota { get; set; }

        // ---------- Datos del turno ----------

        [Required(ErrorMessage = "Debe indicar la fecha y hora del turno.")]
        [Display(Name = "Fecha y hora")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo del turno.")]
        [StringLength(200)]
        [Display(Name = "Motivo del turno")]
        public string Motivo { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de turno: Peluquería o Atención veterinaria.
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar el tipo de turno.")]
        [StringLength(30)]
        [Display(Name = "Tipo de turno")]
        public string TipoTurno { get; set; } = "Atención veterinaria";

        /// <summary>
        /// Estado del turno: Pendiente, Confirmado, Atendido, Cancelado.
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Estado del turno")]
        public string EstadoTurno { get; set; } = "Pendiente";

        // ---------- Veterinario asignado (usuario Identity) ----------

        [Display(Name = "Veterinario asignado")]
        public string? VeterinarioAsignadoId { get; set; }

        // [ForeignKey(nameof(VeterinarioAsignadoId))]
        // public ApplicationUser? VeterinarioAsignado { get; set; }

        // ---------- Extras ----------

        [StringLength(500)]
        [Display(Name = "Notas de recepción")]
        public string? NotasRecepcion { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
