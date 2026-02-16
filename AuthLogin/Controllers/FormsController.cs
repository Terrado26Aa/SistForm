using AuthLogin.Data;
using AuthLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                        Type = item.Type
                    });
                }
            }

            //Guarda todo de una vez
            _context.CForms.Add(newForm);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Formulario creado exitosamente", FormId = newForm.IdForm });
        }

        //Get: api/Forms/all (para listar los formularios)
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

        //Get: api/Forms/{id} (para obtener un formulario por su Id, incluyendo sus elementos)
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
            public List<ResponseDetailDto> Answer { get; set; }
        }

        public class ResponseDetailDto
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitResponse([FromBody] SubmitResponseDto data)
        {
            var response = new CFormResponse
            {
                FormId = data.FormId,
                UserId = data.UserId,
                Date = DateTime.Now
            };

            foreach (var ans in data.Answer)
            {
                response.Details.Add(new CFormResponseDetail
                {
                    QuestionTitle = ans.Question,
                    Answer = ans.Answer
                });
            }

            _context.FormResponses.Add(response);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Respuesta enviada exitosamente" });
        }
    }
}