using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeterinariaElCeibo.Models; // <-- importante

namespace VeterinariaElCeibo.Data
{
    // Ahora el contexto usa ApplicationUser como tipo de usuario
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Vacunacion> Vacunaciones { get; set; }
        public DbSet<Desparasitacion> Desparasitaciones { get; set; }

    }
}


