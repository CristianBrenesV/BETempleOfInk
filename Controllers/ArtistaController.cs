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
    public class ArtistasController : ControllerBase
    {
        private readonly string _connectionString;

        public ArtistasController(IConfiguration configuration)
        {
            // Obtén la cadena de conexión desde el archivo appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET api/artistas
        [HttpGet]
        public ActionResult<IEnumerable<Artista>> GetArtistasC()
        {
            List<Artista> artistas = new List<Artista>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ArtistaIdNombre", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Artista artista = new Artista
                                {
                                    IdArtista = reader.GetInt32(0), // Primer columna: IdArtista
                                    Nombre = reader.GetString(1) // Segunda columna: NombreCompleto
                                };
                                artistas.Add(artista);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return Ok(artistas);
        }
    }
}
