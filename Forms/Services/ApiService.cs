using System.Net.Http.Json;
using Forms.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Forms.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Config.BaseApiUrl);
            
            // Configurar opciones JSON para manejar indistintamente mayúsculas/minúsculas
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task AddAuthHeader()
        {
            string token = DeviceInfo.Platform == DevicePlatform.MacCatalyst 
                ? Preferences.Default.Get("auth_token", "") 
                : await SecureStorage.Default.GetAsync("auth_token");

            _httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token) 
                ? new AuthenticationHeaderValue("Bearer", token) 
                : null;
        }

        // Detecta un 401 y dispara el logout automático
        private static async Task<bool> HandleUnauthorized(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await AppShell.ForceLogoutAsync();
                return true;
            }
            return false;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto logindata)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", logindata);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(_jsonOptions);
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                        {
                            Preferences.Default.Set("auth_token", loginResponse.Token);
                            Preferences.Default.Set("user_id", loginResponse.UserId.ToString());
                            Preferences.Default.Set("user_role", loginResponse.Role ?? "User");
                        }
                        else
                        {
                            await SecureStorage.Default.SetAsync("auth_token", loginResponse.Token);
                            await SecureStorage.Default.SetAsync("user_id", loginResponse.UserId.ToString());
                            await SecureStorage.Default.SetAsync("user_role", loginResponse.Role ?? "User");
                        }
                        return Result<LoginResponseDto>.Success(loginResponse);
                    }
                    return Result<LoginResponseDto>.Failure("Respuesta inválida del servidor");
                }
                
                var error = await response.Content.ReadAsStringAsync();
                return Result<LoginResponseDto>.Failure(string.IsNullOrEmpty(error) ? "Usuario o contraseña incorrectos" : error);
            }
            catch (Exception ex)
            {
                return Result<LoginResponseDto>.Failure($"Error de conexión: {ex.Message}");
            }
        }

        public async Task<Result> RegisterAsync(RegisterRequestDto registerData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerData);
                if (response.IsSuccessStatusCode) return Result.Success();
                
                var error = await response.Content.ReadAsStringAsync();
                return Result.Failure(error);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error de conexión: {ex.Message}");
            }
        }

        public async Task<Result> SaveFormAsync(FormDto formData)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PostAsJsonAsync("api/forms", formData);
                if (await HandleUnauthorized(response)) return Result.Failure("Sesión expirada");
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo guardar el formulario");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateFormAsync(int id, FormDto formDto)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/forms/{id}", formDto);
                if (await HandleUnauthorized(response)) return Result.Failure("Sesión expirada");
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo actualizar el formulario");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> DeleteFormAsync(int id)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.DeleteAsync($"api/forms/{id}");
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo eliminar el formulario");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<List<FormDto>>> GetAllFormsAsync()
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.GetAsync("api/forms/all");
                if (await HandleUnauthorized(response)) return Result<List<FormDto>>.Failure("Sesión expirada");
                if (!response.IsSuccessStatusCode) return Result<List<FormDto>>.Failure("Error al cargar formularios");
                var forms = await response.Content.ReadFromJsonAsync<List<FormDto>>(_jsonOptions);
                return Result<List<FormDto>>.Success(forms ?? new List<FormDto>());
            }
            catch (Exception ex)
            {
                return Result<List<FormDto>>.Failure($"Error al cargar formularios: {ex.Message}");
            }
        }

        public async Task<Result<FormDto>> GetFormDetailsAsync(int id)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.GetAsync($"api/forms/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var form = await response.Content.ReadFromJsonAsync<FormDto>(_jsonOptions);
                    return form != null ? Result<FormDto>.Success(form) : Result<FormDto>.Failure("Contenido vacío");
                }
                return Result<FormDto>.Failure("Formulario no encontrado");
            }
            catch (Exception ex)
            {
                return Result<FormDto>.Failure(ex.Message);
            }
        }

        public async Task<Result> SubmitResponseAsync(SubmitResponseDto responseDto)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PostAsJsonAsync("api/forms/submit", responseDto);
                if (await HandleUnauthorized(response)) return Result.Failure("Sesión expirada");
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("Error al enviar la respuesta");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<UserProfileDto>> GetUserProfileAsync(int userId)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.GetAsync($"api/auth/profile/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>(_jsonOptions);
                    return profile != null ? Result<UserProfileDto>.Success(profile) : Result<UserProfileDto>.Failure("Datos nulos");
                }
                return Result<UserProfileDto>.Failure("No se pudo obtener el perfil");
            }
            catch (Exception ex)
            {
                return Result<UserProfileDto>.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateUserProfileAsync(int userId, UserProfileDto profileData)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/auth/profile/{userId}", profileData);
                if (response.IsSuccessStatusCode) return Result.Success();
                
                var error = await response.Content.ReadAsStringAsync();
                return Result.Failure(error);
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<List<SubmitResponseDto>>> GetFormResultsAsync(int id)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.GetAsync($"api/forms/{id}/results");
                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadFromJsonAsync<List<SubmitResponseDto>>(_jsonOptions);
                    return Result<List<SubmitResponseDto>>.Success(results ?? new List<SubmitResponseDto>());
                }
                return Result<List<SubmitResponseDto>>.Failure("No se pudieron cargar los resultados");
            }
            catch (Exception ex)
            {
                return Result<List<SubmitResponseDto>>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<UserDto>>> GetUsersAsync()
        {
            try
            {
                await AddAuthHeader();
                var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/forms/users", _jsonOptions);
                return Result<List<UserDto>>.Success(users ?? new List<UserDto>());
            }
            catch (Exception ex)
            {
                return Result<List<UserDto>>.Failure(ex.Message);
            }
        }

        public async Task<Result> AssignFormAsync(int formId, int userId)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PostAsJsonAsync("api/forms/assign", new { formId, userId });
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo asignar la encuesta");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> RegisterAdminAsync(RegisterRequestDto registerData)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PostAsJsonAsync("api/auth/register-admin", registerData);
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo crear el usuario");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateUserRoleAsync(int userId, string newRole)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/auth/users/{userId}/role", newRole);
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo actualizar el rol");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> DeleteUserAsync(int userId)
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.DeleteAsync($"api/auth/users/{userId}");
                return response.IsSuccessStatusCode ? Result.Success() : Result.Failure("No se pudo eliminar el usuario");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.GetAsync("api/forms/dashboard-stats");
                if (await HandleUnauthorized(response)) return Result<DashboardStatsDto>.Failure("Sesión expirada");
                if (!response.IsSuccessStatusCode) return Result<DashboardStatsDto>.Failure("No se pudieron cargar las estadísticas");
                var stats = await response.Content.ReadFromJsonAsync<DashboardStatsDto>(_jsonOptions);
                return Result<DashboardStatsDto>.Success(stats ?? new DashboardStatsDto());
            }
            catch (Exception ex)
            {
                return Result<DashboardStatsDto>.Failure(ex.Message);
            }
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
