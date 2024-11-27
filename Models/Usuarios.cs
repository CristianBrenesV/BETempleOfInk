namespace BETempleOfInk.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Contraseña { get; set; }
        public string? TipoUsuario { get; set; } 
        public bool Miembro { get; set; }
        public DateTime? MiembroDesde { get; set; }
        public string? NivelMembresia { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }
}
