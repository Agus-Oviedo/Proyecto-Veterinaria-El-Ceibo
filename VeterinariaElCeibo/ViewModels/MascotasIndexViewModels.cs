using System.Collections.Generic;
using VeterinariaElCeibo.Models;

namespace VeterinariaElCeibo.ViewModels
{
    public class MascotasIndexViewModel
    {
        // El tutor
        public Cliente Cliente { get; set; }

        // Todas las mascotas de ese tutor
        public List<Mascota> Mascotas { get; set; } = new();
    }
}
