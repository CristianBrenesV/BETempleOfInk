namespace BETempleOfInk.Models
{
    public class InteraccionChatbot
    {
        public int IdInteraccion { get; set; }
        public int? IDUsuario { get; set; }
        public int? IdPregunta { get; set; }
        public string? Respuesta { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
