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
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET: api/Galeria/Publicados
        [HttpGet("Publicados")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Galeria>>> GetGaleriaPublicados()
        {
            try
            {
                var galerias = new List<Galeria>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("SP_ObtenerGaleriaPublicados", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            galerias.Add(new Galeria
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

                return Ok(galerias);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error en el servidor al obtener las galerías publicadas.");
            }
        }

        // GET: api/Galeria/PorArtista/{idArtista}
        [HttpGet("PorArtista/{idArtista}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Galeria>>> GetGaleriaPorArtista(int idArtista)
        {
            try
            {
                var galerias = new List<Galeria>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ObtenerGaleriaPorArtista", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdArtista", idArtista);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            galerias.Add(new Galeria
                            {
                                IdTatuaje = reader.GetInt32(0),
                                NombreTatuaje = reader.GetString(1),
                                ImagenTatuaje = reader.GetString(2),
                                FechaPublicacion = reader.GetDateTime(3),
                                Publicar = reader.GetByte(4)
                            });
                        }
                    }
                }

                if (galerias.Count == 0)
                {
                    return NotFound($"No se encontraron galerías para el artista con ID {idArtista}.");
                }

                return Ok(galerias);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error en el servidor al obtener las galerías por artista.");
            }
        }

        // POST: api/Galeria
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertGaleria(Galeria nuevaGaleria)
        {
            try
            {
                if (nuevaGaleria == null)
                {
                    return BadRequest("Los datos de la galería son inválidos.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_InsertarGaleria", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@NombreTatuaje", nuevaGaleria.NombreTatuaje);
                    command.Parameters.AddWithValue("@ImagenTatuaje", nuevaGaleria.ImagenTatuaje);
                    command.Parameters.AddWithValue("@FechaPublicacion", nuevaGaleria.FechaPublicacion);
                    command.Parameters.AddWithValue("@IdArtista", nuevaGaleria.IdArtista);
                    command.Parameters.AddWithValue("@Publicar", nuevaGaleria.Publicar);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return CreatedAtAction(nameof(GetGaleriaPublicados), nuevaGaleria);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error en el servidor al insertar la galería.");
            }
        }

        // PUT: api/Galeria/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGaleria(int id, Galeria galeriaActualizada)
        {
            try
            {
                if (id != galeriaActualizada.IdTatuaje)
                {
                    return BadRequest("El ID proporcionado no coincide con el ID de la galería.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_ActualizarGaleria", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@IdTatuaje", id);
                    command.Parameters.AddWithValue("@NombreTatuaje", galeriaActualizada.NombreTatuaje);
                    command.Parameters.AddWithValue("@ImagenTatuaje", galeriaActualizada.ImagenTatuaje);
                    command.Parameters.AddWithValue("@FechaPublicacion", galeriaActualizada.FechaPublicacion);
                    command.Parameters.AddWithValue("@IdArtista", galeriaActualizada.IdArtista);
                    command.Parameters.AddWithValue("@Publicar", galeriaActualizada.Publicar);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"No se encontró ninguna galería con ID {id}.");
                    }
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error en el servidor al actualizar la galería.");
            }
        }
    }
}
