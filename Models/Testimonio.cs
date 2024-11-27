namespace BETempleOfInk.Models
{
    public class Testimonio
    {
        public int IdTestimonio { get; set; }
        public string? Nombre { get; set; }
        public string? TestimonioTexto { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public byte? Publicar { get; set; }
        public int? IdUsuario { get; set; }
    }
}
