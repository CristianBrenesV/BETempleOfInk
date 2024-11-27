namespace BETempleOfInk.Models
{
    public class AgendaArtista
    {
        public int IdAgenda { get; set; }
        public int IdArtista { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool? Disponible { get; set; }
        public bool? EsMembresia { get; set; }
    }
}
