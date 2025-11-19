using System.Collections.Generic;

namespace VeterinariaElCeibo.Models
{
    public class UsuarioRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class RolSeleccionadoViewModel
    {
        public string Nombre { get; set; }
        public bool Seleccionado { get; set; }
    }

    public class EditarRolesUsuarioViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<RolSeleccionadoViewModel> Roles { get; set; }
    }
}
