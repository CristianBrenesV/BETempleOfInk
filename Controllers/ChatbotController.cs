using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using BETempleOfInk.Models;

namespace BETempleOfInk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ChatbotController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Método para registrar interacciones
        [HttpPost("RegistrarInteraccion")]
        public async Task<IActionResult> RegistrarInteraccion([FromBody] Chatbot chatbot)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_InteraccionesChatbotRegistrar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Pregunta", chatbot.Pregunta ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Respuesta", chatbot.Respuesta ?? (object)DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { message = "Interacción registrada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Método para obtener opciones del menú según el rol
        [HttpGet("ObtenerOpcionesMenu/{rol}")]
        public async Task<IActionResult> ObtenerOpcionesMenu(string rol)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_OpcionesMenuObtener", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Rol", rol);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var opciones = new List<OpcionesMenu>();

                            while (await reader.ReadAsync())
                            {
                                opciones.Add(new OpcionesMenu
                                {
                                    IdOpcion = Convert.ToInt32(reader["IdOpcion"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    Activo = Convert.ToBoolean(reader["Activo"]),
                                    Rol = reader["Rol"].ToString()
                                });
                            }

                            return Ok(opciones);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
