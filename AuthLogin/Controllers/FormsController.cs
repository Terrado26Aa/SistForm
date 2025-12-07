using AuthLogin.Data;
using AuthLogin.Models;
using Microsoft.AspNetCore.Mvc;

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

            var newForm = new CForm
            {
                IdUser = createFormDto.IdUser,
                Title = createFormDto.Title,
                Description = createFormDto.Description,
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };

            _context.CForms.Add(newForm);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Formulario creado exitosamente", FormId = newForm.IdForm });
        }
    }
}