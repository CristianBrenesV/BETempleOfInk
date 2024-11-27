namespace BETempleOfInk.Models
{
    public class Cotizacion
    {
        public int CotizacionId { get; set; }
        public int ClienteId { get; set; }
        public string? Descripcion { get; set; }
        public decimal? PrecioEstimado { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public string? Estado { get; set; }
        public int ArtistaId { get; set; }
    }
}
