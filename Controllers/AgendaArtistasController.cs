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

        //GET: api/AgendaArtista     Listo
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AgendaArtista>>> Get_All_AgendaArtistas()
        {
            try
            {
                var agendaArtistas = new List<AgendaArtista>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_AgendaArtistaObtenerTodos", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            agendaArtistas.Add(new AgendaArtista
                            {
                                IdAgenda = reader.GetInt32(0),
                                IdArtista = reader.GetString(1), 
                                Fecha = reader.GetDateTime(2),
                                HoraInicio = reader.GetTimeSpan(3),
                                HoraFin = reader.GetTimeSpan(4),
                                Disponible = reader.GetBoolean(5),
                                EsMembresia = reader.GetBoolean(6),
                                Publicar = reader.GetBoolean(7)
                            });
                        }
                    }
                }

                return Ok(agendaArtistas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
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

                    command.Parameters.AddWithValue("@IdArtista", idArtista );
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
                                IdArtista = reader.GetInt32(1).ToString(), // Convierte el número a string
                                Fecha = reader.GetDateTime(2),
                                HoraInicio = reader.GetTimeSpan(3),
                                HoraFin = reader.GetTimeSpan(4),
                                Disponible = reader.GetBoolean(5),
                                EsMembresia = reader.GetBoolean(6),
                                Publicar = reader.GetBoolean(7)
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

        //GET: api/AgendaArtista/{idAgenda}  Obtener por Id
        [HttpGet("{idAgenda}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<AgendaArtista>> GetAgendaById(int idAgenda)
        {
            // Validación de entrada
            if (idAgenda <= 0)
            {
                return BadRequest("El ID de la agenda no es válido.");
            }

            try
            {
                // Conexión y ejecución del SP
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_AgendaArtistaObtenerPorId", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add(new SqlParameter("@IdAgenda", idAgenda));

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var agenda = new AgendaArtista
                            {
                                IdAgenda = reader.GetInt32(0),
                                IdArtista = reader.GetInt32(1).ToString(), 
                                Fecha = reader.GetDateTime(2),
                                HoraInicio = reader.GetTimeSpan(3),
                                HoraFin = reader.GetTimeSpan(4),
                                Disponible = reader.GetBoolean(5),
                                EsMembresia = reader.GetBoolean(6),
                                Publicar = reader.GetBoolean(7)
                            };

                            return Ok(agenda); 
                        }
                        else
                        {
                            return NotFound(); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en el servidor: {ex.Message}");
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
                    var command = new SqlCommand("sp_AgendaArtistaInsertar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Parámetros con tipos explícitos
                    command.Parameters.AddWithValue("@IdArtista", nuevaAgenda.IdArtista);
                    command.Parameters.AddWithValue("@Fecha", nuevaAgenda.Fecha);
                    command.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = nuevaAgenda.HoraInicio;
                    command.Parameters.Add("@HoraFin", SqlDbType.Time).Value = nuevaAgenda.HoraFin;
                    command.Parameters.AddWithValue("@Disponible", nuevaAgenda.Disponible.HasValue ? (object)nuevaAgenda.Disponible.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@EsMembresia", nuevaAgenda.EsMembresia.HasValue ? (object)nuevaAgenda.EsMembresia.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Publicar", nuevaAgenda.Publicar.HasValue ? (object)nuevaAgenda.Publicar.Value : DBNull.Value);
                    
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetDisponibilidad), new { idArtista = nuevaAgenda.IdArtista }, nuevaAgenda);
            }
            catch (Exception ex)
            {
                // Log error details
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al crear la agenda del artista: {ex.Message}");
            }
        }

        //PUT: api/AgendaArtista/{idAgenda} Actualizar
        [HttpPut("{idAgenda}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarAgendaArtista(
            int idAgenda,
            [FromBody] AgendaArtista ActualizarAgenda)
        {
            if (ActualizarAgenda == null)
            {
                return BadRequest("El modelo de datos no puede ser nulo.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("sp_AgendaArtistaActualizar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros del procedimiento almacenado
                        command.Parameters.AddWithValue("@IdAgenda", idAgenda);
                        command.Parameters.AddWithValue("@Fecha", ActualizarAgenda.Fecha == default ? DBNull.Value : ActualizarAgenda.Fecha);
                        command.Parameters.AddWithValue("@HoraInicio", ActualizarAgenda.HoraInicio == default ? DBNull.Value : ActualizarAgenda.HoraInicio);
                        command.Parameters.AddWithValue("@HoraFin", ActualizarAgenda.HoraFin == default ? DBNull.Value : ActualizarAgenda.HoraFin);
                        command.Parameters.AddWithValue("@Disponible", ActualizarAgenda.Disponible ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EsMembresia", ActualizarAgenda.EsMembresia ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Publicar", ActualizarAgenda.Publicar ?? (object)DBNull.Value);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { Message = "Agenda actualizada correctamente." });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }


        // Pactch: api/AgendaArtista/Actualizar/{idAgenda}     Listo (Disponible y membresia)
        [HttpPatch("Actualizar/{idAgenda}")]
        [ProducesResponseType(StatusCodes.Status200OK)]  
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateAgenda(int idAgenda, [FromBody] ActualizarAgendaArtista requestData)
        {
            // Verificamos si el requestData es nulo o si el campo 'Campo' está vacío.
            if (requestData == null || string.IsNullOrEmpty(requestData.Campo))
            {
                return BadRequest(new { message = "El campo 'Campo' es obligatorio." });
            }

            try
            {
                // Ejecutamos la actualización directamente aquí
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_AgendaArtistaActualizarPublicarOMembresiaODisponible", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@idAgenda", idAgenda);
                    command.Parameters.AddWithValue("@Campo", requestData.Campo);
                    command.Parameters.AddWithValue("@Valor", requestData.Valor);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Agenda actualizada correctamente." });  // Respuesta en JSON
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Error al actualizar la agenda." });  // Respuesta en JSON
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });  // Respuesta en JSON
            }
        }

    }
}
