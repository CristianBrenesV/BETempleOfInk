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
    public class BeneficiosController : ControllerBase
    {
        private readonly string _connectionString;

        public BeneficiosController(IConfiguration configuration)
        {
            // Obtén la cadena de conexión desde el archivo appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
        }

        // GET api/beneficios
        [HttpGet]
        public ActionResult<IEnumerable<Beneficios>> GetBeneficios()
        {
            List<Beneficios> beneficios = new List<Beneficios>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_BeneficiosIdNombre", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Beneficios beneficio = new Beneficios
                                {
                                    IdBeneficio = reader.GetInt32(0), 
                                    Nombre = reader.GetString(1) 
                                };
                                beneficios.Add(beneficio);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return Ok(beneficios);
        }
    }
}
