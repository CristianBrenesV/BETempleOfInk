namespace BETempleOfInk.Models
{
    public class GaleriaConSubcategoriasDTO
    {
        public int IdTatuaje { get; set; }
        public string? NombreTatuaje { get; set; }
        public string? ImagenTatuaje { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int IdArtista { get; set; }
        public bool? Publicar { get; set; }
        public string? Subcategorias { get; set; }
    }
}