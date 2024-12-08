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
    public class TestimoniosController : ControllerBase
    {
        private readonly string _connectionString;

        public TestimoniosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/Testimonios/Ultimos
        [HttpGet("Ultimos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUltimosTestimonios()
        {
            try
            {
                var testimonios = new List<Testimonios>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TestimoniosObtenerUltimos", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            testimonios.Add(new Testimonios
                            {
                                IdTestimonio = reader.GetInt32(reader.GetOrdinal("IdTestimonio")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                TestimonioTexto = reader.GetString(reader.GetOrdinal("Testimonio")),
                                FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("FechaPublicacion")),
                                Calificacion = reader.GetInt32(reader.GetOrdinal("Calificacion")),
                                Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar")),
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario"))
                            });
                        }
                    }
                }

                if (testimonios.Any())
                {
                    return Ok(testimonios);
                }
                else
                {
                    return NotFound("No se encontraron testimonios.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Testimonios
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTestimonios()
        {
            try
            {
                var testimonios = new List<Testimonios>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TestimoniosObtener", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            testimonios.Add(new Testimonios
                            {
                                IdTestimonio = reader.GetInt32(reader.GetOrdinal("IdTestimonio")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                TestimonioTexto = reader.GetString(reader.GetOrdinal("Testimonio")),
                                FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("FechaPublicacion")),
                                Calificacion = reader.GetInt32(reader.GetOrdinal("Calificacion")),
                                Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar")),
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario"))
                            });
                        }
                    }
                }

                return Ok(testimonios);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Testimonios
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostTestimonio([FromBody] Testimonios testimonio)
        {
            if (testimonio.Calificacion < 1 || testimonio.Calificacion > 5)
            {
                return BadRequest("La calificación debe estar entre 1 y 5.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TestimoniosInsertar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Testimonio", testimonio.TestimonioTexto);
                    command.Parameters.AddWithValue("@IdUsuario", testimonio.IdUsuario);
                    command.Parameters.AddWithValue("@Calificacion", testimonio.Calificacion);
                    command.Parameters.AddWithValue("@Publicar", testimonio.Publicar);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return StatusCode(StatusCodes.Status201Created, new { message = "Testimonio creado correctamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al insertar el testimonio." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }

        // PATCH: api/Testimonios/Actualizar/{idTestimonio}
        [HttpPatch("Actualizar/{idTestimonio}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTestimonio(int idTestimonio, [FromBody] ActualizarTestimonio requestData)
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
                    var command = new SqlCommand("sp_TestimoniosActualizarPublicar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTestimonio", idTestimonio);
                    command.Parameters.AddWithValue("@Campo", requestData.Campo);
                    command.Parameters.AddWithValue("@Valor", requestData.Valor);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Testimonio actualizado correctamente." }); // Respuesta en JSON
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Error al actualizar el testimonio." }); // Respuesta en JSON
                    }
                }
            }
            catch (SqlException ex)
            {
                // Manejo de errores de SQL
                return StatusCode(500, new { message = $"Error de base de datos: {ex.Message}" });
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }




        // GET: api/Testimonios/{idTestimonio}
        [HttpGet("{idTestimonio}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTestimonioById(int idTestimonio)
        {
            // Validación de entrada
            if (idTestimonio <= 0)
            {
                return BadRequest("El ID del testimonio no es válido.");
            }

            try
            {
                Testimonios testimonio = new Testimonios();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TestimoniosObtenerPorId", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add(new SqlParameter("@idTestimonio", idTestimonio));

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Llenar los valores del objeto Testimonios
                            testimonio.IdTestimonio = reader.GetInt32(reader.GetOrdinal("IdTestimonio"));
                            testimonio.Nombre = reader.GetString(reader.GetOrdinal("Nombre"));
                            testimonio.TestimonioTexto = reader.GetString(reader.GetOrdinal("TestimonioTexto"));
                            testimonio.FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("FechaPublicacion"));
                            testimonio.Calificacion = reader.GetInt32(reader.GetOrdinal("Calificacion"));
                            testimonio.Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar"));
                            testimonio.IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario"));
                        }
                    }
                }

                if (testimonio != null)
                {
                    return Ok(testimonio);
                }
                else
                {
                    return NotFound("No se encontró el testimonio.");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en el servidor: {ex.Message}");
            }
        }

        // DELETE: api/Testimonios/{idTestimonio}
        [HttpDelete("{idTestimonio}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTestimonio(int idTestimonio)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TestimoniosEliminar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTestimonio", idTestimonio);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en el servidor: {ex.Message}");
            }
        }
        
        // DELETE: api/Testimonios/EliminarPorPalabrasVetadas
        [HttpDelete("EliminarPorPalabrasVetadas")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> EliminarTestimoniosPorPalabrasVetadas()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TestimoniosEliminarPorPalabrasVetadas", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    int eliminados = await command.ExecuteNonQueryAsync();

                    return Ok(new { Mensaje = $"Número de testimonios eliminados: {eliminados}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"Error en el servidor: {ex.Message}" });
            }
        }
    }
}
