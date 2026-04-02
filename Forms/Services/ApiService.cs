using System.Net.Http.Json;
using Forms.Models;
using System.Net.Http.Headers;

namespace Forms.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly string BaseApiUrl = DetermineBaseUrl();

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseApiUrl);
        }

        private static string DetermineBaseUrl()
        {
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return "http://10.0.2.2:5174";
            }

            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return "http://TU_IP_DE_MAC_O_PC:5174";
            }

            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return "http://localhost:5174";
            }

            return "http://localhost:5174";

#endif
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto logindata)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/auth/login", logindata);

                //Exito, Deserializamos y devolvemos los datos
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        // Guardar el token en SecureStorage
                        await SecureStorage.Default.SetAsync("auth_token", loginResponse.Token);

                        await SecureStorage.Default.SetAsync("user_id", loginResponse.UserId.ToString());

                        await Application.Current.MainPage.DisplayAlert("Token Guardado", $"El token empieza con: " +
                            $"{loginResponse.Token.Substring(0, 15)}... \nID Usuario: {loginResponse.UserId}", "OK");
                    }

                    return loginResponse;
                }
                else
                {
                    //fallo, lanzamos una excepcion con la informacion del error.
                    //la vista se encarga de atrapar la excepcion.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error de la API: {response.StatusCode} - {errorContent}", null, response.StatusCode);
                }
            }

            catch (HttpRequestException httpEx)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw new Exception($"Ha ocurrido un error inesperado en el servidor: {ex.Message}");
            }
        }
        public async Task RegisterAsync(RegisterRequestDto registerData)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/register", registerData);
                if (!response.IsSuccessStatusCode)
                {
                    //Si la API devuelve un error, lo lanzamos.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(errorContent, null, response.StatusCode);
                }
                //Si es exitoso, no necesitamos devolver nada.
            }
            catch (HttpRequestException)
            {
                //Relanzamos la excepción  para que la UI la maneje
                throw;
            }
            catch (Exception ex)
            {
                //Envolvemos calquier oto error
                throw new Exception($"Ocurrio un error inesperado en el registro: {ex.Message}");
            }
        }

        public async Task<bool> SaveFormAsync(FormDto formData)
        {
            try
            {
                // Obtener el token de SecureStorage
                var token = await SecureStorage.Default.GetAsync("auth_token");

                // Agregar el token al encabezado de autorización
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("api/forms", formData);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error api al guardar: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Error de la API: {response.StatusCode} - {errorContent}", null, response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al guardar el formulario: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFormAsync(int id)
        {
            try
            {
                // Obtener el token de SecureStorage
                var token = await SecureStorage.Default.GetAsync("auth_token");
                // Agregar el token al encabezado de autorización
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //endpoint para eliminar un formulario por id
                var response = await _httpClient.DeleteAsync($"api/forms/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al eliminar el formulario: {ex.Message}");
            }
        }

        public async Task<bool> UpdateFormAsync(int id, FormDto formDto)
        {
            try
            {
                // Obtener el token de SecureStorage
                var token = await SecureStorage.Default.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //endpoint para actualizar un formulario por id
                var response = await _httpClient.PutAsJsonAsync($"api/forms/{id}", formDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al actualizar el formulario: {ex.Message}");
                return false;
            }
        }

        //logica que devuelve todos los formularios
        public async Task<List<FormDto>> GetAllFormsAsync()
        {
            try
            {
                // Obtener el token de SecureStorage
                var token = await SecureStorage.Default.GetAsync("auth_token");

                // Agregar el token al encabezado de autorización
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //endpoint para obtener todos los formularios
                return await _httpClient.GetFromJsonAsync<List<FormDto>>("api/forms/all") ?? new List<FormDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener los formularios: {ex.Message}");
                return new List<FormDto>(); //En caso de error, devolvemos una lista vacía para evitar que la aplicación se caiga.
            }
        }

        public async Task<FormDto> GetFormDetailsAsync(int id)
        {
            try
            {
                // Obtener el token de SecureStorage
                var token = await SecureStorage.Default.GetAsync("auth_token");
                // Agregar el token al encabezado de autorización
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"api/forms/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var form = await response.Content.ReadFromJsonAsync<FormDto>();
                    return form ?? new FormDto();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error de la API: {response.StatusCode} - {errorContent}", null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al obtener los detalles del formulario: {ex.Message}");
            }
        }

        public async Task<bool> SubmitResponseAsync(SubmitResponseDto responseDto)
        {
            try
            {
                // Obtener el token de SecureStorage
                var token = await SecureStorage.Default.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                //Envia los datos
                var httpResponse = await _httpClient.PostAsJsonAsync("api/forms/submit", responseDto);

                return httpResponse.IsSuccessStatusCode;
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar la respuesta: {ex.Message}");
                return false;
            }
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/auth/profile/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserProfileDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UserProfileDto profileData)
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.PutAsJsonAsync($"api/auth/profile/{userId}", profileData);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
