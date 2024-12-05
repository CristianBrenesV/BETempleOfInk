namespace BETempleOfInk.Models
{
    public class MembresiaDto
    {
        public string? Nivel { get; set; }
        public decimal PrecioMensual { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Duracion { get; set; }
        public bool Publicar { get; set; }
        public List<int> BeneficiosIds { get; set; }
    }

}
