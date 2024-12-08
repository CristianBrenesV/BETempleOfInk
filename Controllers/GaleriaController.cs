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
    public class GaleriaController : ControllerBase
    {
        private readonly string _connectionString;

        public GaleriaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/Galeria
        [HttpGet]
        public async Task<IActionResult> GetGaleria()
        {
            try
            {
                var tatuajes = new List<Galeria>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConSubcategoriasObtener", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tatuajes.Add(new Galeria
                            {
                                IdTatuaje = reader.GetInt32(0),
                                NombreTatuaje = reader.GetString(1),
                                ImagenTatuaje = reader.GetString(2),
                                FechaPublicacion = reader.GetDateTime(3),
                                IdArtista = reader.GetInt32(4),
                                Publicar = reader.GetByte(5)
                            });
                        }
                    }
                }

                return Ok(tatuajes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Galeria/{idTatuaje}
        [HttpGet("{idTatuaje}")]
        public async Task<IActionResult> GetGaleriaById(int idTatuaje)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConSubcategoriasObtenerPorId", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@IdTatuaje", idTatuaje);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var tatuaje = new Galeria
                            {
                                IdTatuaje = reader.GetInt32(0),
                                NombreTatuaje = reader.GetString(1),
                                ImagenTatuaje = reader.GetString(2),
                                FechaPublicacion = reader.GetDateTime(3),
                                IdArtista = reader.GetInt32(4),
                                Publicar = reader.GetByte(5)
                            };

                            return Ok(tatuaje);
                        }
                        else
                        {
                            return NotFound("Tatuaje no encontrado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Galeria
        [HttpPost]
        public async Task<IActionResult> CreateGaleria([FromBody] Galeria galeria, [FromQuery] string subcategoriasIds)
        {
            if (galeria == null)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConFiltrosInsertar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@NombreTatuaje", galeria.NombreTatuaje);
                    command.Parameters.AddWithValue("@ImagenTatuaje", galeria.ImagenTatuaje);
                    command.Parameters.AddWithValue("@FechaPublicacion", galeria.FechaPublicacion);
                    command.Parameters.AddWithValue("@IdArtista", galeria.IdArtista);
                    command.Parameters.AddWithValue("@Publicar", galeria.Publicar);
                    command.Parameters.AddWithValue("@SubcategoriasIds", subcategoriasIds);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return StatusCode(StatusCodes.Status201Created, "Tatuaje creado correctamente.");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error al insertar el tatuaje.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Galeria/{idTatuaje}
        [HttpPut("{idTatuaje}")]
        public async Task<IActionResult> UpdateGaleria(int idTatuaje, [FromBody] Galeria galeria, [FromQuery] string subcategoriasIds)
        {
            if (galeria == null)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConFiltrosActualizar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTatuaje", idTatuaje);
                    command.Parameters.AddWithValue("@NombreTatuaje", galeria.NombreTatuaje);
                    command.Parameters.AddWithValue("@ImagenTatuaje", galeria.ImagenTatuaje);
                    command.Parameters.AddWithValue("@FechaPublicacion", galeria.FechaPublicacion);
                    command.Parameters.AddWithValue("@IdArtista", galeria.IdArtista);
                    command.Parameters.AddWithValue("@Publicar", galeria.Publicar);
                    command.Parameters.AddWithValue("@SubcategoriasIds", subcategoriasIds);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return Ok("Tatuaje actualizado correctamente.");
                    }
                    else
                    {
                        return NotFound("Tatuaje no encontrado.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PATCH: api/Galeria/{idTatuaje}
        [HttpPatch("{idTatuaje}")]
        public async Task<IActionResult> UpdatePublicar(int idTatuaje, [FromBody] ActualizarGaleria actualizacion)
        {
            if (actualizacion == null || string.IsNullOrEmpty(actualizacion.Campo))
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesActualizarPublicar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTatuaje", idTatuaje);
                    command.Parameters.AddWithValue("@Campo", actualizacion.Campo);
                    command.Parameters.AddWithValue("@Valor", actualizacion.Valor);

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return Ok("Estado de publicación actualizado correctamente.");
                    }
                    else
                    {
                        return NotFound("Tatuaje no encontrado.");
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
