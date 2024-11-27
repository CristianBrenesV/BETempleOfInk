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
    public class JoinGaleriaCategoriaController : ControllerBase
    {
        private readonly string _connectionString;

        public JoinGaleriaCategoriaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/JoinGaleriaCategoria
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<JoinGaleriaCategoria>>> GetAllJoinGaleriaCategoria()
        {
            try
            {
                var items = new List<JoinGaleriaCategoria>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ConsultarTodosJoinGaleriaCategoria", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new JoinGaleriaCategoria
                            {
                                IdGaleriaCategoria = reader.GetInt32(0),
                                IdTatuaje = reader.GetInt32(1),
                                IdCategoria = reader.GetInt32(2)
                            });
                        }
                    }
                }

                return Ok(items);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los datos de JoinGaleriaCategoria.");
            }
        }

        // GET: api/JoinGaleriaCategoria/Categorias/{idTatuaje}
        [HttpGet("Categorias/{idTatuaje}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCategoriasPorTatuaje(int idTatuaje)
        {
            try
            {
                var categorias = new List<dynamic>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenerCategoriasPorTatuaje", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTatuaje", idTatuaje);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categorias.Add(new
                            {
                                IdGaleriaCategoria = reader.GetInt32(0),
                                IdCategoria = reader.GetInt32(1),
                                CategoriaNombre = reader.GetString(2)
                            });
                        }
                    }
                }

                if (categorias.Count == 0)
                {
                    return NotFound($"No se encontraron categorías para el tatuaje con ID {idTatuaje}.");
                }

                return Ok(categorias);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las categorías del tatuaje.");
            }
        }

        // GET: api/JoinGaleriaCategoria/Galeria/{idCategoria}
        [HttpGet("Galeria/{idCategoria}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetGaleriaPorCategoria(int idCategoria)
        {
            try
            {
                var galeria = new List<dynamic>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenerGaleriaPorCategoria", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdCategoria", idCategoria);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            galeria.Add(new
                            {
                                IdGaleriaCategoria = reader.GetInt32(0),
                                IdTatuaje = reader.GetInt32(1),
                                NombreTatuaje = reader.GetString(2)
                            });
                        }
                    }
                }

                if (galeria.Count == 0)
                {
                    return NotFound($"No se encontraron tatuajes para la categoría con ID {idCategoria}.");
                }

                return Ok(galeria);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los tatuajes de la categoría.");
            }
        }

        // GET: api/JoinGaleriaCategoria/{idGaleriaCategoria}
        [HttpGet("{idGaleriaCategoria}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JoinGaleriaCategoria>> GetJoinGaleriaCategoria(int idGaleriaCategoria)
        {
            try
            {
                // Verificar si el idGaleriaCategoria es válido.
                if (idGaleriaCategoria <= 0)
                {
                    return BadRequest("El ID de la relación de galería y categoría no es válido.");
                }

                JoinGaleriaCategoria item = new JoinGaleriaCategoria();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenerJoinGaleriaCategoriaPorId", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdGaleriaCategoria", idGaleriaCategoria);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new JoinGaleriaCategoria
                            {
                                IdGaleriaCategoria = reader.GetInt32(0),
                                IdTatuaje = reader.GetInt32(1),
                                IdCategoria = reader.GetInt32(2)
                            };
                        }
                    }
                }

                if (item == null)
                {
                    return NotFound($"No se encontró la relación para ID {idGaleriaCategoria}.");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener la relación de galería y categoría. {ex.Message}");
            }
        }


        // POST: api/JoinGaleriaCategoria
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(JoinGaleriaCategoria nuevaRelacion)
        {
            try
            {
                if (nuevaRelacion == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_InsertarJoinGaleriaCategoria", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTatuaje", nuevaRelacion.IdTatuaje);
                    command.Parameters.AddWithValue("@IdCategoria", nuevaRelacion.IdCategoria);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetJoinGaleriaCategoria), new { idGaleriaCategoria = nuevaRelacion.IdGaleriaCategoria }, nuevaRelacion);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear la relación de galería y categoría.");
            }
        }

        // PUT: api/JoinGaleriaCategoria
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(JoinGaleriaCategoria actualizacionRelacion)
        {
            try
            {
                if (actualizacionRelacion == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ActualizarJoinGaleriaCategoria", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdGaleriaCategoria", actualizacionRelacion.IdGaleriaCategoria);
                    command.Parameters.AddWithValue("@IdTatuaje", actualizacionRelacion.IdTatuaje);
                    command.Parameters.AddWithValue("@IdCategoria", actualizacionRelacion.IdCategoria);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar la relación de galería y categoría.");
            }
        }
    }
}
