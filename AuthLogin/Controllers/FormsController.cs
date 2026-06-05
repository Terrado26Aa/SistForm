using AuthLogin.Models;
using AuthLogin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AuthLogin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        private readonly IFormsService _formsService;

        public FormsController(IFormsService formsService)
        {
            _formsService = formsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm([FromBody] CreateFormDto createFormDto)
        {
            var result = await _formsService.CreateFormAsync(createFormDto);
            return Ok(new { Message = result.message, FormId = result.formId });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var result = await _formsService.DeleteFormAsync(id);
            if (!result.success) return NotFound(new { Message = result.message });

            return Ok(new { Message = result.message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] CreateFormDto updateDto)
        {
            var result = await _formsService.UpdateFormAsync(id, updateDto);
            if (!result.success) return NotFound(new { Message = result.message });

            return Ok(new { Message = result.message });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllForms()
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userRole == "Admin")
            {
                var forms = await _formsService.GetAllFormsAsync();
                return Ok(forms);
            }
            else
            {
                var forms = await _formsService.GetAssignedFormsAsync(userId);
                return Ok(forms);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormById(int id)
        {
            var form = await _formsService.GetFormByIdAsync(id);
            if (form == null) return NotFound("Formulario no encontrado");
            return Ok(form);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitResponse([FromBody] SubmitResponseDto data)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int tokenUserId))
            {
                if (data.UserId == 0 && tokenUserId != 0)
                {
                    data.UserId = tokenUserId;
                }
            }

            var result = await _formsService.SubmitResponseAsync(data);
            if (!result.success) return BadRequest(result.message);

            return Ok(new { Message = result.message });
        }

        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetResults(int id)
        {
            var results = await _formsService.GetResponsesByFormIdAsync(id);
            return Ok(results);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignForm([FromBody] FormAssignmentDto data)
        {
            var result = await _formsService.AssignFormAsync(data.FormId, data.UserId);
            if (!result.success) return BadRequest(new { Message = result.message });
            return Ok(new { Message = result.message });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _formsService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalForms = await _formsService.GetTotalFormsCountAsync();
            var totalUsers = await _formsService.GetTotalUsersCountAsync();
            var totalResponses = await _formsService.GetTotalResponsesCountAsync();
            var recentForms = await _formsService.GetRecentFormsAsync(5);

            return Ok(new
            {
                TotalForms = totalForms,
                TotalUsers = totalUsers,
                TotalResponses = totalResponses,
                RecentForms = recentForms
            });
        }
    }
}