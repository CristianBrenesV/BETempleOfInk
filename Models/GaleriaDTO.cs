namespace BETempleOfInk.Models
{
    public class GaleriaDTO
    {
        public int IdTatuaje { get; set; }
        public string? NombreTatuaje { get; set; }
        public string? ImagenTatuaje { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int IdArtista { get; set; }
        public bool? Publicar { get; set; }
        public List<int> SubcategoriaIds { get; set; }
    }

}
