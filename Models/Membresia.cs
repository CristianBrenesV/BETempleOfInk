namespace BETempleOfInk.Models
{
    public class Membresia
    {
        public int IdMembresia { get; set; }
        public string? Nivel { get; set; }
        public decimal? PrecioMensual { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int? Duracion { get; set; }
        public bool Publicar { get; set; }
        
    }
}
