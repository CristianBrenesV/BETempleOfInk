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
    public class JoinMembresiaBeneficioController : ControllerBase
    {
        private readonly string _connectionString;

        public JoinMembresiaBeneficioController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }
/*
        // GET: api/JoinMembresiaBeneficio/Membresia/{idMembresia}
        [HttpGet("Membresia/{idMembresia}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<JoinMembresiaBeneficio>>> GetByMembresia(int idMembresia)
        {
            try
            {
                var beneficios = new List<JoinMembresiaBeneficio>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenertJoinMembresiaBeneficioPorMembresia", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdMembresia", idMembresia);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            beneficios.Add(new JoinMembresiaBeneficio
                            {
                                IdJoinMembresiaBeneficio = reader.GetInt32(0),
                                IdMembresia = reader.GetInt32(1),
                                IdBeneficio = reader.GetInt32(2)
                            });
                        }
                    }
                }

                if (beneficios.Count == 0)
                {
                    return NotFound($"No se encontraron beneficios para la membresía con ID {idMembresia}.");
                }

                return Ok(beneficios);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los beneficios de la membresía.");
            }
        }

        // GET: api/JoinMembresiaBeneficio
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<JoinMembresiaBeneficio>>> GetAll()
        {
            try
            {
                var beneficios = new List<JoinMembresiaBeneficio>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ConsultarTodasMembresiaBeneficio", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            beneficios.Add(new JoinMembresiaBeneficio
                            {
                                IdJoinMembresiaBeneficio = reader.GetInt32(0),
                                IdMembresia = reader.GetInt32(1),
                                IdBeneficio = reader.GetInt32(2)
                            });
                        }
                    }
                }

                return Ok(beneficios);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la lista de beneficios.");
            }
        }

        // POST: api/JoinMembresiaBeneficio
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(JoinMembresiaBeneficio nuevoBeneficio)
        {
            try
            {
                if (nuevoBeneficio == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_InsertarJoinMembresiaBeneficio", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdMembresia", nuevoBeneficio.IdMembresia);
                    command.Parameters.AddWithValue("@IdBeneficio", nuevoBeneficio.IdBeneficio);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetByMembresia), new { idMembresia = nuevoBeneficio.IdMembresia }, nuevoBeneficio);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al agregar el beneficio a la membresía.");
            }
        }

        // PUT: api/JoinMembresiaBeneficio/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, JoinMembresiaBeneficio beneficioActualizado)
        {
            try
            {
                if (beneficioActualizado == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ActualizarJoinMembresiaBeneficio", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdJoinMembresiaBeneficio", id);
                    command.Parameters.AddWithValue("@IdMembresia", beneficioActualizado.IdMembresia);
                    command.Parameters.AddWithValue("@IdBeneficio", beneficioActualizado.IdBeneficio);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"No se encontró el registro con ID {id}.");
                    }
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el registro.");
            }
        }*/
    }
}
