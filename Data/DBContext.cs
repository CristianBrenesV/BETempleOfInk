using Microsoft.EntityFrameworkCore;
using BETempleOfInk.Models;

namespace BETempleOfInk.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }

        public DbSet<Usuario>? Usuarios { get; set; }
        public DbSet<Testimonios>? Testimonios { get; set; }
        public DbSet<OpcionesMenu>? OpcionesMenus { get; set; }
        public DbSet<AgendaArtista>? AgendaArtistas { get; set; }
        public DbSet<Artista>? Artistas { get; set; }
        public DbSet<Beneficios>? Beneficios { get; set; }
        public DbSet<Categoria>? Categorias { get; set; }
        public DbSet<Cotizacion>? Cotizaciones { get; set; }
        public DbSet<Galeria>? Galerias { get; set; }
        public DbSet<ImagenArticulo>? ImagenesArticulos { get; set; }
        public DbSet<InteraccionChatbot>? InteraccionesChatbot { get; set; }
        public DbSet<JoinGaleriaCategoria>? JoinGaleriaCategorias { get; set; }
        public DbSet<JoinMembresiaBeneficio>? JoinMembresiaBeneficios { get; set; }
        public DbSet<JoinUsuarioMembresia>? JoinUsuarioMembresias { get; set; }
        public DbSet<Membresia>? Membresias { get; set; }
        public DbSet<Chatbot>? Chatbots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JoinGaleriaCategoria>()
                .HasKey(j => new { j.IdTatuaje, j.IdCategoria });

            modelBuilder.Entity<JoinMembresiaBeneficio>()
                .HasKey(j => new { j.IdMembresia, j.IdBeneficio });

            modelBuilder.Entity<JoinUsuarioMembresia>()
                .HasKey(j => new { j.IdUsuario, j.IdMembresia });
        }
    }

}
