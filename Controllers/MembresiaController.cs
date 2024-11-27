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
    public class MembresiasController : ControllerBase
    {
        private readonly string _connectionString;

        public MembresiasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/Membresias
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Membresia>>> GetAllMembresias()
        {
            try
            {
                var membresias = new List<Membresia>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ConsultarTodasMembresias", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            membresias.Add(new Membresia
                            {
                                IdMembresia = reader.GetInt32(0),
                                Nivel = reader.GetString(1),
                                PrecioMensual = reader.GetDecimal(2),
                                FechaCreacion = reader.GetDateTime(3),
                                FechaVencimiento = reader.GetDateTime(4),
                                Duracion = reader.GetInt32(5),
                                Publicar = reader.GetByte(6)
                            });
                        }
                    }
                }

                return Ok(membresias);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las membresías.");
            }
        }

        // GET: api/Membresias/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Membresia>> GetMembresiaById(int id)
        {
            try
            {
                Membresia? membresia = null;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ConsultarMembresia", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@idMembresia", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            membresia = new Membresia
                            {
                                IdMembresia = reader.GetInt32(0),
                                Nivel = reader.GetString(1),
                                PrecioMensual = reader.GetDecimal(2),
                                FechaCreacion = reader.GetDateTime(3),
                                FechaVencimiento = reader.GetDateTime(4),
                                Duracion = reader.GetInt32(5),
                                Publicar = reader.GetByte(6)
                            };
                        }
                    }
                }

                if (membresia == null)
                {
                    return NotFound($"No se encontró la membresía con ID {id}.");
                }

                return Ok(membresia);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la membresía.");
            }
        }

        // POST: api/Membresias
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMembresia(Membresia nuevaMembresia)
        {
            try
            {
                if (nuevaMembresia == null)
                {
                    return BadRequest("Los datos de la membresía son inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_InsertarMembresia", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Nivel", nuevaMembresia.Nivel);
                    command.Parameters.AddWithValue("@PrecioMensual", nuevaMembresia.PrecioMensual);
                    command.Parameters.AddWithValue("@FechaCreacion", nuevaMembresia.FechaCreacion);
                    command.Parameters.AddWithValue("@FechaVencimiento", nuevaMembresia.FechaVencimiento);
                    command.Parameters.AddWithValue("@Duracion", nuevaMembresia.Duracion);
                    command.Parameters.AddWithValue("@Publicar", nuevaMembresia.Publicar);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetMembresiaById), new { id = nuevaMembresia.IdMembresia }, nuevaMembresia);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear la membresía.");
            }
        }

        // PUT: api/Membresias/{id}/Publicar
        [HttpPut("{id}/Publicar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePublicarMembresia(int id, [FromBody] byte publicar)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ActualizarPublicarMembresia", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@idMembresia", id);
                    command.Parameters.AddWithValue("@Publicar", publicar);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"No se encontró la membresía con ID {id}.");
                    }
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el estado de publicación.");
            }
        }
    }
}
