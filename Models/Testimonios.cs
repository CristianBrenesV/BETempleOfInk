namespace BETempleOfInk.Models
{
    public class Testimonios
    {
        public int IdTestimonio { get; set; }
        public string? Nombre { get; set; }
        public string? TestimonioTexto { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public int? Calificacion { get; set; }
        public bool? Publicar { get; set; }
        public int? IdUsuario { get; set; }
    }
}
