using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BETempleOfInk.Models;

namespace BETempleOfInk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendaArtistaController : ControllerBase
    {
        private readonly string _connectionString;

        public AgendaArtistaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/AgendaArtista/{idArtista}/Disponibilidad
        [HttpGet("{idArtista}/Disponibilidad")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AgendaArtista>>> GetDisponibilidad(
            int idArtista, 
            [FromQuery] DateTime? fechaInicio, 
            [FromQuery] DateTime? fechaFin)
        {
            try
            {
                if (!fechaInicio.HasValue && !fechaFin.HasValue)
                {
                    return BadRequest("Debe proporcionar al menos una fecha (FechaInicio o FechaFin).");
                }

                var agendas = new List<AgendaArtista>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("ObtenerAgendaArtistaDisponibilidad", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdArtista", idArtista);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin ?? (object)DBNull.Value);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            agendas.Add(new AgendaArtista
                            {
                                IdAgenda = reader.GetInt32(0),
                                IdArtista = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2),
                                HoraInicio = reader.GetTimeSpan(3),
                                HoraFin = reader.GetTimeSpan(4),
                                Disponible = reader.GetBoolean(5),
                                EsMembresia = reader.GetBoolean(6)
                            });
                        }
                    }
                }

                return Ok(agendas);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la disponibilidad del artista.");
            }
        }

        // POST: api/AgendaArtista
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAgenda(AgendaArtista nuevaAgenda)
        {
            try
            {
                if (nuevaAgenda == null)
                {
                    return BadRequest("Los datos de la agenda son inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_InsertarAgendaArtista", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdArtista", nuevaAgenda.IdArtista);
                    command.Parameters.AddWithValue("@Fecha", nuevaAgenda.Fecha);
                    command.Parameters.AddWithValue("@HoraInicio", nuevaAgenda.HoraInicio);
                    command.Parameters.AddWithValue("@HoraFin", nuevaAgenda.HoraFin);
                    command.Parameters.AddWithValue("@Disponible", nuevaAgenda.Disponible);
                    command.Parameters.AddWithValue("@EsMembresia", nuevaAgenda.EsMembresia);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetDisponibilidad), new { idArtista = nuevaAgenda.IdArtista }, nuevaAgenda);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear la agenda del artista.");
            }
        }

        // PUT: api/AgendaArtista/{idAgenda}
        [HttpPut("{idAgenda}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAgenda(int idAgenda, AgendaArtista actualizacionAgenda)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ActualizarAgendaArtista", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdAgenda", idAgenda);
                    command.Parameters.AddWithValue("@Fecha", actualizacionAgenda.Fecha == default ? (object)DBNull.Value : actualizacionAgenda.Fecha);
                    command.Parameters.AddWithValue("@HoraInicio", actualizacionAgenda.HoraInicio == default ? (object)DBNull.Value : actualizacionAgenda.HoraInicio);
                    command.Parameters.AddWithValue("@HoraFin", actualizacionAgenda.HoraFin == default ? (object)DBNull.Value : actualizacionAgenda.HoraFin);
                    command.Parameters.AddWithValue("@Disponible", actualizacionAgenda.Disponible);
                    command.Parameters.AddWithValue("@EsMembresia", actualizacionAgenda.EsMembresia);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"No se encontró la agenda con ID {idAgenda}.");
                    }
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar la agenda del artista.");
            }
        }
    }
}
