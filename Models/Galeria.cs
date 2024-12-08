namespace BETempleOfInk.Models
{
    public class Galeria
    {
        public int IdTatuaje { get; set; }
        public string? NombreTatuaje { get; set; }
        public string? ImagenTatuaje { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int IdArtista { get; set; }
        public byte Publicar { get; set; }
        
    }
}
