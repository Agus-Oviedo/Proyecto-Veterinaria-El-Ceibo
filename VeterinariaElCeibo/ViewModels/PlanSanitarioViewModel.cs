using System.Collections.Generic;
using VeterinariaElCeibo.Models;

namespace VeterinariaElCeibo.ViewModels
{
    public class PlanSanitarioViewModel
    {
        public Mascota Mascota { get; set; }
        public List<Vacunacion> Vacunaciones { get; set; } = new();
        public List<Desparasitacion> Desparasitaciones { get; set; } = new();
    }
}
