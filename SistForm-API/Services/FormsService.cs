using SistFormAPI.Data;
using SistFormAPI.Models;
using Microsoft.EntityFrameworkCore;
using GeoJSON.Net.Geometry;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;

namespace SistFormAPI.Services
{
    public class FormsService : IFormsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormsService> _logger;

        public FormsService(ApplicationDbContext context, ILogger<FormsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool success, string message, int formId)> CreateFormAsync(CreateFormDto createFormDto)
        {
            var newForm = new CForm
            {
                IdUser = createFormDto.IdUser,
                Title = createFormDto.Title,
                Description = createFormDto.Description,
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Elements = createFormDto.Elements.Select(e => new CFormElement
                {
                    Title = e.Title,
                    Type = e.Type,
                    Options = e.Options,
                    MaxSelections = e.MaxSelections
                }).ToList()
            };

            _context.CForms.Add(newForm);
            await _context.SaveChangesAsync();
            return (true, "Formulario creado exitosamente", newForm.IdForm);
        }

        public async Task<(bool success, string message)> DeleteFormAsync(int id)
        {
            var form = await _context.CForms.FindAsync(id);
            if (form == null) return (false, "Formulario no encontrado");

            _context.CForms.Remove(form);
            await _context.SaveChangesAsync();
            return (true, "Formulario eliminado exitosamente");
        }

        public async Task<(bool success, string message)> UpdateFormAsync(int id, CreateFormDto updateDto)
        {
            var form = await _context.CForms.Include(f => f.Elements).FirstOrDefaultAsync(f => f.IdForm == id);
            if (form == null) return (false, "Formulario no encontrado");

            form.Title = updateDto.Title;
            form.Description = updateDto.Description;
            form.LastModifiedDate = DateTime.Now;

            // Lógica de actualización granular para preservar integridad
            var existingElements = form.Elements.ToList();
            var updatedElementIds = new HashSet<int>();

            foreach (var elementDto in updateDto.Elements)
            {
                if (elementDto.Id.HasValue)
                {
                    var existing = existingElements.FirstOrDefault(e => e.Id == elementDto.Id.Value);
                    if (existing != null)
                    {
                        existing.Title = elementDto.Title;
                        existing.Type = elementDto.Type;
                        existing.Options = elementDto.Options;
                        existing.MaxSelections = elementDto.MaxSelections;
                        updatedElementIds.Add(existing.Id);
                    }
                }
                else
                {
                    // Es un elemento nuevo
                    form.Elements.Add(new CFormElement
                    {
                        Title = elementDto.Title,
                        Type = elementDto.Type,
                        Options = elementDto.Options,
                        MaxSelections = elementDto.MaxSelections,
                        FormId = id
                    });
                }
            }

            // Opcional: Eliminar elementos que no vinieron en el DTO 
            // (Solo si no tienen respuestas asociadas, o podrías marcarlos como inactivos)
            var elementsToRemove = existingElements.Where(e => !updatedElementIds.Contains(e.Id)).ToList();
            _context.FormElements.RemoveRange(elementsToRemove);

            await _context.SaveChangesAsync();
            return (true, "Formulario actualizado exitosamente");
        }

        public async Task<IEnumerable<object>> GetAllFormsAsync()
        {
            return await _context.CForms
                .Select(f => new
                {
                    f.IdForm,
                    f.Title,
                    f.Description,
                })
                .ToListAsync();
        }

        public async Task<CForm?> GetFormByIdAsync(int id)
        {
            return await _context.CForms
                .Include(f => f.Elements)
                .FirstOrDefaultAsync(f => f.IdForm == id);
        }

        public async Task<(bool success, string message)> SubmitResponseAsync(SubmitResponseDto data)
        {
            if (data == null || data.Responses == null) return (false, "Datos invalidos o vacios");

            string? finalRouteJson = null;
            if (data.TrackPoints != null && data.TrackPoints.Count >= 2)
            {
                var locations = data.TrackPoints.Select(p => new Position(p.Lat, p.Lon)).ToList();
                var lineString = new LineString(locations);
                var feature = new Feature(lineString);
                finalRouteJson = JsonConvert.SerializeObject(feature);
            }

            var response = new CFormResponse
            {
                FormId = data.FormId,
                UserId = data.UserId,
                Date = DateTime.Now,
                LatitudeA = data.LatitudeA,
                LongitudeA = data.LongitudeA,
                LatitudeB = data.LatitudeB,
                LongitudeB = data.LongitudeB,
                RoutePath = finalRouteJson,
                Details = data.Responses.Select(ans => new CFormResponseDetail
                {
                    QuestionTitle = ans.Question ?? "Sin título",
                    Answer = ans.Answer ?? "Sin respuesta"
                }).ToList()
            };

            _context.FormResponses.Add(response);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Respuesta recibida para FormId {FormId} del UserId {UserId}", data.FormId, data.UserId);
            return (true, "Respuesta enviada exitosamente");
        }

        public async Task<IEnumerable<CFormResponse>> GetResponsesByFormIdAsync(int formId)
        {
            return await _context.FormResponses
                .Include(r => r.Details)
                .Where(r => r.FormId == formId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<(bool success, string message)> AssignFormAsync(int formId, int userId)
        {
            if (await _context.FormAssignments.AnyAsync(a => a.FormId == formId && a.UserId == userId))
                return (false, "La encuesta ya está asignada a este usuario");

            _context.FormAssignments.Add(new FormAssignment { FormId = formId, UserId = userId });
            await _context.SaveChangesAsync();
            return (true, "Encuesta asignada exitosamente");
        }

        public async Task<IEnumerable<object>> GetAssignedFormsAsync(int userId)
        {
            return await _context.FormAssignments
                .Where(a => a.UserId == userId)
                .Include(a => a.Form)
                .Select(a => new
                {
                    a.Form.IdForm,
                    a.Form.Title,
                    a.Form.Description
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new { u.Id, u.UserName, u.FirstName, u.LastName, u.Role })
                .ToListAsync();
        }

        public async Task<int> GetTotalFormsCountAsync()
        {
            return await _context.CForms.CountAsync();
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalResponsesCountAsync()
        {
            return await _context.FormResponses.CountAsync();
        }

        public async Task<IEnumerable<object>> GetRecentFormsAsync(int count)
        {
            return await _context.CForms
                .OrderByDescending(f => f.CreationDate)
                .Take(count)
                .Select(f => new
                {
                    f.IdForm,
                    f.Title,
                    f.Description,
                    f.CreationDate
                })
                .ToListAsync();
        }
    }
}
