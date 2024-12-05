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

        // GET: api/MembresiasConBeneficios
        [HttpGet("MembresiasConBeneficios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMembresiasConBeneficios()
        {
            try
            {
                var membresiasConBeneficios = new List<MembresiaConBeneficiosDto>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_MembresiasConBeneficiosObtener", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
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


        // PUT: api/MembresiasActualizar/{id}
        [HttpPut("MembresiasActualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutMembresia(int id, [FromBody] MembresiaDto membresiaDto)
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

                    command.Parameters.AddWithValue("@IdMembresia", id);
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
