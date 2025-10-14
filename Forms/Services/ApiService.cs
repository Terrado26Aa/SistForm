using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Forms.Models;
using Microsoft.Maui.Devices;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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
            if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                return "http://10.0.2.2:5096";
            }

            else if(DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return "http://TU_IP_DE_MAC_O_PC:5096";
            }

            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return "http://localhost:5096";
            }

            return "http://localhost:5096";

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
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
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
    }
}
