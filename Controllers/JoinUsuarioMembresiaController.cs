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
    public class JoinUsuarioMembresiaController : ControllerBase
    {
        private readonly string _connectionString;

        public JoinUsuarioMembresiaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }
/*
        // GET: api/JoinUsuarioMembresia/Activas/{idUsuario}
        [HttpGet("Activas/{idUsuario}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<JoinUsuarioMembresia>>> GetActiveMemberships(int idUsuario)
        {
            try
            {
                var membresias = new List<JoinUsuarioMembresia>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenerMembresiasActivas", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            membresias.Add(new JoinUsuarioMembresia
                            {
                                IdUsuario = idUsuario,
                                IdMembresia = reader.GetInt32(0),
                                FechaInicio = reader.GetDateTime(1),
                                FechaFin = reader.GetDateTime(2),
                                FechaRenovacion = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                Activo = reader.GetBoolean(4),
                                Renovacion = reader.GetBoolean(5)
                            });
                        }
                    }
                }

                if (membresias.Count == 0)
                {
                    return NotFound($"No se encontraron membresías activas para el usuario con ID {idUsuario}.");
                }

                return Ok(membresias);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las membresías activas.");
            }
        }

        // GET: api/JoinUsuarioMembresia/ProximasRenovacion/{diasAntes}
        [HttpGet("ProximasRenovacion/{diasAntes}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<JoinUsuarioMembresia>>> GetMembershipsNearRenewal(int diasAntes)
        {
            try
            {
                var membresias = new List<JoinUsuarioMembresia>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenerMembresiasProximasRenovacion", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@DiasAntes", diasAntes);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            membresias.Add(new JoinUsuarioMembresia
                            {
                                IdUsuario = reader.GetInt32(0),
                                IdMembresia = reader.GetInt32(1),
                                FechaInicio = reader.GetDateTime(2),
                                FechaFin = reader.GetDateTime(3),
                                FechaRenovacion = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Activo = true, // Renovaciones siempre activas
                                Renovacion = false
                            });
                        }
                    }
                }

                return Ok(membresias);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener membresías próximas a renovación.");
            }
        }

       // POST: api/JoinUsuarioMembresia
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(JoinUsuarioMembresia nuevaMembresia)
        {
            try
            {
                if (nuevaMembresia == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_InsertarUsuarioMembresia", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdUsuario", nuevaMembresia.IdUsuario);
                    command.Parameters.AddWithValue("@IdMembresia", nuevaMembresia.IdMembresia);
                    command.Parameters.AddWithValue("@FechaInicio", nuevaMembresia.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", nuevaMembresia.FechaFin);
                    command.Parameters.AddWithValue("@FechaRenovacion", 
                        nuevaMembresia.FechaRenovacion.HasValue ? (object)nuevaMembresia.FechaRenovacion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", nuevaMembresia.Activo);
                    command.Parameters.AddWithValue("@Renovacion", nuevaMembresia.Renovacion);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetActiveMemberships), new { idUsuario = nuevaMembresia.IdUsuario }, nuevaMembresia);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear la membresía para el usuario.");
            }
        }


        // PUT: api/JoinUsuarioMembresia
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(JoinUsuarioMembresia membresiaActualizada)
        {
            try
            {
                if (membresiaActualizada == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ActualizarUsuarioMembresia", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdUsuario", membresiaActualizada.IdUsuario);
                    command.Parameters.AddWithValue("@IdMembresia", membresiaActualizada.IdMembresia);
                    command.Parameters.AddWithValue("@FechaInicio", membresiaActualizada.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", membresiaActualizada.FechaFin);
                    command.Parameters.AddWithValue("@FechaRenovacion", 
                        membresiaActualizada.FechaRenovacion.HasValue ? (object)membresiaActualizada.FechaRenovacion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", membresiaActualizada.Activo);
                    command.Parameters.AddWithValue("@Renovacion", membresiaActualizada.Renovacion);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar la membresía.");
            }
        }*/

    }
}
