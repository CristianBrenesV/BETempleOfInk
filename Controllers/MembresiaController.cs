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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMembresiasConBeneficios()
        {
            try
            {
                var membresiasConBeneficios = new List<MembresiaConBeneficiosDto>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Define el comando para ejecutar el SP
                    var command = new SqlCommand("sp_MembresiasConBeneficiosObtener", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();

                    // Ejecuta el lector de datos
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            membresiasConBeneficios.Add(new MembresiaConBeneficiosDto
                            {
                                IdMembresia = reader.GetInt32(reader.GetOrdinal("idMembresia")),
                                Nivel = reader.GetString(reader.GetOrdinal("Nivel")),
                                PrecioMensual = reader.GetDecimal(reader.GetOrdinal("PrecioMensual")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
                                Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                                Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar")),
                                Beneficios = reader.GetString(reader.GetOrdinal("Beneficios"))
                            });
                        }
                    }
                }

                // Si hay datos, se retornan como respuesta
                if (membresiasConBeneficios.Any())
                {
                    return Ok(membresiasConBeneficios);
                }
                else
                {
                    return NotFound("No se encontraron membresías con beneficios.");
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        
        // POST: api/MembresiasCrear
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostMembresia([FromBody] MembresiaDto membresiaDto)
        {
            if (membresiaDto == null)
            {
                return BadRequest(new { message = "Los datos de la membresía son inválidos." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_MembresiaConBeneficiosInsertar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Nivel", membresiaDto.Nivel);
                    command.Parameters.AddWithValue("@PrecioMensual", membresiaDto.PrecioMensual);
                    command.Parameters.AddWithValue("@FechaCreacion", membresiaDto.FechaCreacion);
                    command.Parameters.AddWithValue("@FechaVencimiento", membresiaDto.FechaVencimiento);
                    command.Parameters.AddWithValue("@Duracion", membresiaDto.Duracion);
                    command.Parameters.AddWithValue("@Publicar", membresiaDto.Publicar);
                    command.Parameters.AddWithValue("@BeneficiosIds", string.Join(",", membresiaDto.BeneficiosIds));  

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return StatusCode(StatusCodes.Status201Created, new { message = "Membresía creada correctamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al insertar la membresía." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }

        // PATCH: api/Membresias/Actualizar/{idMembresia}
        [HttpPatch("Actualizar/{idMembresia}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMembresia(int idMembresia, [FromBody] ActualizarMembresia requestData)
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
                    var command = new SqlCommand("sp_MembresiaActualizarPublicar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdMembresia", idMembresia);
                    command.Parameters.AddWithValue("@Campo", requestData.Campo);
                    command.Parameters.AddWithValue("@Valor", requestData.Valor);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Membresía actualizada correctamente." }); // Respuesta en JSON
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Error al actualizar la membresía." }); // Respuesta en JSON
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


        // GET: api/Membresias/{idMembresia} Obtener membresía con beneficios por Id
        [HttpGet("{idMembresia}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<MembresiaConBeneficiosDto>> GetMembresiaConBeneficiosById(int idMembresia)
        {
            // Validación de entrada
            if (idMembresia <= 0)
            {
                return BadRequest("El ID de la membresía no es válido.");
            }

            try
            {
                // Conexión y ejecución del SP
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_MembresiaConBeneficiosObtenerPorId", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add(new SqlParameter("@idMembresia", idMembresia));

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Crear el DTO para devolver los datos
                            var membresia = new MembresiaConBeneficiosDto
                            {
                                IdMembresia = reader.GetInt32(0),
                                Nivel = reader.GetString(1),
                                PrecioMensual = reader.GetDecimal(2),
                                FechaCreacion = reader.GetDateTime(3),
                                FechaVencimiento = reader.GetDateTime(4),
                                Beneficios = reader.GetString(5), // Beneficios concatenados
                                Duracion = reader.GetInt32(6),
                                Publicar = reader.GetBoolean(7)
                            };

                            return Ok(membresia); // Retornar la membresía con sus beneficios
                        }
                        else
                        {
                            return NotFound(); // Si no se encuentra la membresía
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en el servidor: {ex.Message}");
            }
        }
    

        // PUT: api/MembresiasActualizar/{id}
        [HttpPut("MembresiasActualizar/{idMembresia}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutMembresia(int idMembresia, [FromBody] MembresiaDto membresiaDto)
        {
            if (membresiaDto == null)
            {
                return BadRequest("Los datos de la membresía son inválidos.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_MembresiaConBeneficiosActualizar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdMembresia", idMembresia);
                    command.Parameters.AddWithValue("@Nivel", membresiaDto.Nivel);
                    command.Parameters.AddWithValue("@PrecioMensual", membresiaDto.PrecioMensual);
                    command.Parameters.AddWithValue("@FechaCreacion", membresiaDto.FechaCreacion);
                    command.Parameters.AddWithValue("@FechaVencimiento", membresiaDto.FechaVencimiento);
                    command.Parameters.AddWithValue("@Duracion", membresiaDto.Duracion);
                    command.Parameters.AddWithValue("@Publicar", membresiaDto.Publicar);
                    command.Parameters.AddWithValue("@BeneficiosIds", string.Join(",", membresiaDto.BeneficiosIds));  // Convertir la lista de beneficios en un string separado por comas

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return Ok("Membresía actualizada correctamente.");
                    }
                    else
                    {
                        return NotFound("Membresía no encontrada.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
