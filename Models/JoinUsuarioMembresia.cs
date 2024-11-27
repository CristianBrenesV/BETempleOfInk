namespace BETempleOfInk.Models
{
    public class JoinUsuarioMembresia
    {
        public int IdUsuario { get; set; }
        public int IdMembresia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaRenovacion { get; set; }
        public bool Activo { get; set; }
        public bool Renovacion { get; set; }
    }
}
