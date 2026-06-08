using SistFormAPI.Models;
using static SistFormAPI.Controllers.FormsController;

namespace SistFormAPI.Services
{
    public interface IFormsService
    {
        Task<(bool success, string message, int formId)> CreateFormAsync(CreateFormDto createFormDto);
        Task<(bool success, string message)> DeleteFormAsync(int id);
        Task<(bool success, string message)> UpdateFormAsync(int id, CreateFormDto updateDto);
        Task<IEnumerable<object>> GetAllFormsAsync();
        Task<CForm?> GetFormByIdAsync(int id);
        Task<(bool success, string message)> SubmitResponseAsync(SubmitResponseDto data);
        Task<IEnumerable<CFormResponse>> GetResponsesByFormIdAsync(int formId);
        Task<(bool success, string message)> AssignFormAsync(int formId, int userId);
        Task<IEnumerable<object>> GetAssignedFormsAsync(int userId);
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<int> GetTotalFormsCountAsync();
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetTotalResponsesCountAsync();
        Task<IEnumerable<object>> GetRecentFormsAsync(int count);
    }
}
