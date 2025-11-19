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
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<ConsultaClinica> ConsultasClinicas { get; set; }
        public DbSet<Internacion> Internaciones { get; set; }
        public DbSet<RegistroInternacion> RegistrosInternacion { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Turno>()
                .HasOne(t => t.Mascota)
                .WithMany() // si después le agregás ICollection<Turno> a Mascota, lo cambiamos
                .HasForeignKey(t => t.MascotaId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}


