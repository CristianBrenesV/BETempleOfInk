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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGaleria()
        {
            try
            {
                var tatuajesConSubcategorias = new List<GaleriaConSubcategoriasDTO>();

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
                            tatuajesConSubcategorias.Add(new GaleriaConSubcategoriasDTO
                            {
                                IdTatuaje = reader.GetInt32(reader.GetOrdinal("IdTatuaje")),
                                NombreTatuaje = reader.GetString(reader.GetOrdinal("NombreTatuaje")),
                                ImagenTatuaje = reader.GetString(reader.GetOrdinal("ImagenTatuaje")),
                                FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("FechaPublicacion")),
                                IdArtista = reader.GetInt32(reader.GetOrdinal("IdArtista")),
                                Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar")),
                                Subcategorias = reader.GetString(reader.GetOrdinal("Subcategorias"))
                            });
                        }
                    }
                }

                return Ok(tatuajesConSubcategorias);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }


        // GET: api/Galeria/{idTatuaje}
        [HttpGet("{idTatuaje}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                                Publicar = reader.GetBoolean(5)
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


        // GET: api/Galeria/Subcategorias/{filtroNombre}
        [HttpGet("Subcategorias/{filtroNombre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubcategoriasPorFiltro(string filtroNombre)
        {
            try
            {
                // Inicializa la lista para almacenar las subcategorías recuperadas
                var subcategorias = new List<Subcategoria>();

                // Configura la conexión a la base de datos
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesSubcategoriasPorFiltroObtener", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Agrega el parámetro del filtro al comando
                    command.Parameters.AddWithValue("@FiltroNombre", filtroNombre);

                    // Abre la conexión a la base de datos
                    await connection.OpenAsync();

                    // Ejecuta el comando y procesa el resultado
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            subcategorias.Add(new Subcategoria
                            {
                                IdSubcategoria = reader.GetInt32(0), // Asume que la columna 0 es IdSubcategoria
                                Nombre = reader.GetString(1)         // Asume que la columna 1 es Nombre
                            });
                        }
                    }
                }

                // Si no se encontraron resultados, devuelve un NotFound con un mensaje informativo
                if (subcategorias.Count == 0)
                {
                    return NotFound(new { message = $"No se encontraron subcategorías para el filtro '{filtroNombre}'." });
                }

                // Devuelve las subcategorías como respuesta exitosa (HTTP 200)
                return Ok(subcategorias);
            }
            catch (Exception ex)
            {
                // Manejo de errores: devuelve un estado HTTP 500 con un mensaje del error
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }


        // GET: api/Galeria
        [HttpGet("Inicio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGaleriaInicio()
        {
            try
            {
                var tatuajesConSubcategorias = new List<GaleriaConSubcategoriasDTO>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConSubcategoriasObtenerInicio", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tatuajesConSubcategorias.Add(new GaleriaConSubcategoriasDTO
                            {
                                IdTatuaje = reader.GetInt32(reader.GetOrdinal("IdTatuaje")),
                                NombreTatuaje = reader.GetString(reader.GetOrdinal("NombreTatuaje")),
                                ImagenTatuaje = reader.GetString(reader.GetOrdinal("ImagenTatuaje")),
                                FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("FechaPublicacion")),
                                IdArtista = reader.GetInt32(reader.GetOrdinal("IdArtista")),
                                Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar")),
                                Subcategorias = reader.GetString(reader.GetOrdinal("Subcategorias"))
                            });
                        }
                    }
                }

                return Ok(tatuajesConSubcategorias);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }


        // GET: api/Galeria/Cliente
        [HttpGet("Cliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGaleriaCliente()
        {
            try
            {
                var tatuajesConSubcategorias = new List<GaleriaConSubcategoriasDTO>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConSubcategoriasObtenerCliente", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tatuajesConSubcategorias.Add(new GaleriaConSubcategoriasDTO
                            {
                                IdTatuaje = reader.GetInt32(reader.GetOrdinal("IdTatuaje")),
                                NombreTatuaje = reader.GetString(reader.GetOrdinal("NombreTatuaje")),
                                ImagenTatuaje = reader.GetString(reader.GetOrdinal("ImagenTatuaje")),
                                FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("FechaPublicacion")),
                                IdArtista = reader.GetInt32(reader.GetOrdinal("IdArtista")),
                                Publicar = reader.GetBoolean(reader.GetOrdinal("Publicar")),
                                Subcategorias = reader.GetString(reader.GetOrdinal("Subcategorias"))
                            });
                        }
                    }
                }

                return Ok(tatuajesConSubcategorias);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }


        // POST: api/TatuajesCrear
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostTatuaje([FromBody] GaleriaDTO galeriaDto)
        {
            if (galeriaDto == null)
            {
                return BadRequest(new { message = "Los datos del tatuaje son inválidos." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_TatuajesConFiltrosInsertar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@NombreTatuaje", galeriaDto.NombreTatuaje);
                    command.Parameters.AddWithValue("@ImagenTatuaje", galeriaDto.ImagenTatuaje);
                    command.Parameters.AddWithValue("@FechaPublicacion", galeriaDto.FechaPublicacion);
                    command.Parameters.AddWithValue("@IdArtista", galeriaDto.IdArtista);
                    command.Parameters.AddWithValue("@Publicar", galeriaDto.Publicar);
                    command.Parameters.AddWithValue("@SubcategoriasIds", string.Join(",", galeriaDto.SubcategoriaIds));  // Se esperan IDs separados por comas

                    await connection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return StatusCode(StatusCodes.Status201Created, new { message = "Tatuaje creado correctamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al insertar el tatuaje." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }

        // PUT: api/Galeria/{idTatuaje}
        [HttpPut("{idTatuaje}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGaleria(int idTatuaje, [FromBody] GaleriaDTO galeria)
        {
            if (galeria == null || galeria.SubcategoriaIds == null || !galeria.SubcategoriaIds.Any())
            {
                return BadRequest("Datos inválidos o falta de subcategorías.");
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

                    // Convierte la lista de IDs de subcategorías en una cadena separada por comas
                    var subcategoriaIds = string.Join(",", galeria.SubcategoriaIds);
                    command.Parameters.AddWithValue("@SubcategoriasIds", subcategoriaIds);

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


        // PATCH: api/Galeria/Actualizar/{idTatuaje}
        [HttpPatch("Actualizar/{idTatuaje}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
