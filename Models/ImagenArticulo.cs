namespace BETempleOfInk.Models
{
    public class ImagenArticulo
    {
        public int IdImagenArticulo { get; set; }
        public byte[]? Imagen { get; set; }
        public string? ImagenUrl { get; set; }
        public string? DescripcionCorta { get; set; }
    }
}
