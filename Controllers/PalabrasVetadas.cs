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
    public class PalabrasVetadasController : ControllerBase
    {
        private readonly string _connectionString;

        public PalabrasVetadasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/PalabrasVetadas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPalabrasVetadas()
        {
            try
            {
                var palabras = new List<PalabrasVetadas>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("SELECT ID, Palabra, Fecha FROM PalabrasVetadas", connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            palabras.Add(new PalabrasVetadas
                            {
                                ID = reader.GetInt32(0),
                                Palabra = reader.GetString(1),
                                Fecha = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2)
                            });
                        }
                    }
                }

                if (palabras.Any())
                {
                    return Ok(palabras);
                }
                else
                {
                    return NotFound("No se encontraron palabras vetadas.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PalabrasVetadas/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPalabraVetadaById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID del palabras vetadas no es válido.");
            }

            try
            {
                PalabrasVetadas palabra = new PalabrasVetadas();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("SELECT ID, Palabra, Fecha FROM PalabrasVetadas WHERE ID = @ID", connection)
                    {
                        CommandType = CommandType.Text
                    };
                    command.Parameters.Add(new SqlParameter("@ID", id));

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            palabra = new PalabrasVetadas
                            {
                                ID = reader.GetInt32(0),
                                Palabra = reader.GetString(1),
                                Fecha = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2)
                            };
                        }
                    }
                }

                if (palabra != null)
                {
                    return Ok(palabra);
                }
                else
                {
                    return NotFound("Palabra vetada no encontrada.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/PalabrasVetadas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPalabraVetada([FromBody] PalabrasVetadas palabra)
        {
            if (string.IsNullOrEmpty(palabra.Palabra))
            {
                return BadRequest("El campo Palabra es obligatorio.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_PalabraVetadasInsertar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Palabra", palabra.Palabra);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return StatusCode(StatusCodes.Status201Created, new { message = "Palabra vetada creada correctamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al insertar la palabra vetada." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }

        // PUT: api/PalabrasVetadas/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePalabraVetada(int id, [FromBody] PalabrasVetadas palabra)
        {
            if (id <= 0 || id != palabra.ID || string.IsNullOrEmpty(palabra.Palabra))
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_PalabraVetadasActualizar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@ID", palabra.ID);
                    command.Parameters.AddWithValue("@Palabra", palabra.Palabra);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return Ok(new { message = "Palabra vetada actualizada correctamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al actualizar la palabra vetada." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }


        // DELETE: api/PalabrasVetadas/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePalabraVetada(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_PalabraVetadasEliminar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@ID", id);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound("Palabra vetada no encontrada.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en el servidor: {ex.Message}");
            }
        }
    }
}
