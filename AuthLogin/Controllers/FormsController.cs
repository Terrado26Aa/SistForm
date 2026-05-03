using AuthLogin.Data;
using AuthLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoJSON.Net.Geometry;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;

namespace AuthLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm([FromBody] CreateFormDto createFormDto)
        {
            if (createFormDto == null) return BadRequest("Datos del formulario son requeridos");

            //Crea el formulario padre
            var newForm = new CForm
            {
                IdUser = createFormDto.IdUser,
                Title = createFormDto.Title,
                Description = createFormDto.Description,
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Elements = new List<CFormElement>() //Inicializa la lista
            };

            //Mapea los elementos del DTO a la entidad
            if (createFormDto.Elements != null)
            {
                foreach (var item in createFormDto.Elements)
                {
                    newForm.Elements.Add(new CFormElement
                    {
                        Title = item.Title,
                        Type = item.Type,
                        Options = item.Options,
                        MaxSelections = item.MaxSelections
                    });
                }
            }

            //Guarda todo de una vez
            _context.CForms.Add(newForm);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Formulario creado exitosamente", FormId = newForm.IdForm });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            try
            {
                var form = await _context.CForms.FindAsync(id);
                if (form == null) return NotFound(new { Message = "Formulario no encontrado" });

                //Elimina el formulario y sus elementos relacionados (si hay)
                _context.CForms.Remove(form);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Formulario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el formulario: {ex.Message}");
            }
        }

        //Para actualizar un formulario, se elimina el formulario existente y se crea uno nuevo con los datos actualizados.
        //Esto es más sencillo que intentar actualizar cada campo y elemento individualmente, pero ten en cuenta que esto también
        //eliminará cualquier relación o dato asociado al formulario anterior (como respuestas). Si necesitas mantener esas relaciones,
        //deberías implementar una lógica de actualización más detallada.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] CreateFormDto updateDto)
        {
            try
            {
                //Busca el formulario existente con sus elementos relacionados
                var form = await _context.CForms.Include(f => f.Elements) //Incluye los elementos relacionados
                    .FirstOrDefaultAsync(f => f.IdForm == id);
                if (form == null) return NotFound(new { Message = "Formulario no encontrado" });

                //Actualiza los campos del formulario
                form.Title = updateDto.Title;
                form.Description = updateDto.Description;
                form.LastModifiedDate = DateTime.Now;

                //Elimina las preguntas antiguas
                _context.RemoveRange(form.Elements);

                //Actualiza los elementos (puedes mejorar esta lógica para manejar adiciones/eliminaciones)
                var newAnswers = new List<CFormElement>();
                if (updateDto.Elements != null)
                {
                    foreach (var item in updateDto.Elements)
                    {
                        newAnswers.Add(new CFormElement
                        {
                            Title = item.Title,
                            Type = item.Type,
                            Options = item.Options,
                            FormId = id,
                            MaxSelections = item.MaxSelections
                        });
                    }
                }
                form.Elements = newAnswers;

                await _context.SaveChangesAsync();
                return Ok(new { Message = "Formulario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el formulario: {ex.Message}");
            }
        }

        //Para listar los formularios.
        [HttpGet("all")]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _context.CForms
                .Select(f => new
                {
                    f.IdForm,
                    f.Title,
                    f.Description,
                })
                .ToListAsync();
            return Ok(forms);
        }

        //Para obtener un formulario por su Id, incluyendo sus elementos.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormById(int id)
        {
            try
            {
                var form = await _context.CForms
                .Include(f => f.Elements) //Incluye los elementos relacionados
                .FirstOrDefaultAsync(f => f.IdForm == id);
                if (form == null) return NotFound("Formulario no encontrado");
                return Ok(form);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el formulario: {ex.Message}");
            }
        }

        public class SubmitResponseDto
        {
            public int FormId { get; set; }
            public int UserId { get; set; }
            public List<ResponseDetailDto> Responses { get; set; }

            // Para identificar la respuesta localmente antes de enviarla al servidor
            public string? FormTitle { get; set; }
            public DateTime SaveAt { get; set; } = DateTime.Now;
            public double? LatitudeA { get; set; }
            public double? LongitudeA { get; set; }
            public double? LatitudeB { get; set; }
            public double? LongitudeB { get; set; }

            public List<TrackPoint>? TrackPoints { get; set; } //Lista de puntos para el camino recorrido entre A y B
        }

        public class TrackPoint
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        public class ResponseDetailDto
        {
            public int FormElementId { get; set; }
            public string? Question { get; set; }
            public string? Answer { get; set; }
        }

        //Para enviar las respuestas de un formulario.
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitResponse([FromBody] SubmitResponseDto data)
        {
            try
            {
                //proteccion contra datos vacios.
                if (data == null || data.Responses == null)
                {
                    Console.WriteLine("Datos recibidos son nulos o vacios");
                    return BadRequest("Datos invalidos o vacios");
                }

                //Conversion de la ruta a GeoJSON (si se proporcionan puntos de seguimiento)
                string finalRouteJson = null;
                if (data.TrackPoints != null && data.TrackPoints.Count > 0)
                {
                    //Convierte la lista de puntos a GeoJSON
                    var locations = data.TrackPoints.Select(p => new Position(p.Lat, p.Lon)).ToList();
                    if(locations.Count >= 2)
                    {
                        var lineString = new LineString(locations);
                        var feature = new Feature(lineString);
                        finalRouteJson = JsonConvert.SerializeObject(feature);
                    }
                }

                var response = new CFormResponse
                {
                    FormId = data.FormId,
                    UserId = data.UserId,
                    Date = DateTime.Now, //Usamos la fecha real en la que se guardo offline.
                    LatitudeA = data.LatitudeA,
                    LongitudeA = data.LongitudeA,
                    LatitudeB = data.LatitudeB,
                    LongitudeB = data.LongitudeB,
                    RoutePath = finalRouteJson, //Guarda la ruta en formato GeoJSON
                    Details = new List<CFormResponseDetail>() //Inicializa la lista de detalles
                };

                //Mapea las respuestas que mando el celular.
                foreach (var ans in data.Responses)
                {
                    response.Details.Add(new CFormResponseDetail
                    {
                        QuestionTitle = ans.Question,
                        Answer = ans.Answer
                    });
                }

                _context.FormResponses.Add(response);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Respuesta recibida para FormId {data.FormId} del UserId {data.UserId} con {data.Responses.Count} respuestas.");
                return Ok(new { Message = "Respuesta enviada exitosamente" });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                //Maneja errores específicos de la base de datos, como violaciones de clave foránea o problemas de conexión
                string errorMsg = $"ERROR MySQL (Llave Foránea. ¿Existe el Usuario {data?.UserId}?): {dbEx.InnerException?.Message ?? dbEx.Message}";
                Console.WriteLine(errorMsg);
                return StatusCode(500, errorMsg);
            }
            catch (Exception ex)
            {
                // Maneja cualquier otro tipo de error
                Console.WriteLine($"ERROR GENERAL: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

    }
}