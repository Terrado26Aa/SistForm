using Forms.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Forms.Views;

public partial class Login : ContentPage
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://api.example.com"; // Cambia esto por la URL de tu API

    public Login()
	{
		InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        // Validaciones basicas
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Usuario y contraseña no pueden estar vacios.";
            return;
        }

        // Simula una llamada a la API para autentificación
        var loginData = new { Username = username, Password = password};

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                // Autenticación exitosa
                var responseContent = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                await DisplayAlert("Éxito", $"Login Correcto. Bienvenido", "OK");
                // Guarda el token de alguna manera segura (aqui solo se muestra un ejemplo simple)
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
            else
            {
                // Error en la autenticación
                String errorMesagge = $"Error: {response.StatusCode}";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    errorMesagge = "Credenciales Invalidas";
                } else
                {
                    // Intenta leer un mensaje de error del cuerpo
                    try
                    {
                        var errrorContent = await response.Content.ReadAsStringAsync();
                        errorMesagge = $"Error: {response.StatusCode} - {errrorContent}";
                    } catch (Exception) { }
                }
                ShowError(errorMesagge);
            }
        }
        catch (JsonException JsonEx) 
        {
            // Error al Deserealizar la respuesta
            ShowError($"Error procesando la respuesa: {JsonEx.Message}");
        }
        catch( Exception ex)
        {
            //Otro error inesperado
            ShowError($"Ocurrio un error inesperado: {ex.Message}");
        }
        
    }

    private void ShowError (string Message)
    {
        ErrorLabel.Text = Message;
        ErrorLabel.IsVisible = true;
    }

    private async void OnSigninClicked(object sender, EventArgs e)
    {
        // Navega a la pagina de registro
        await Shell.Current.GoToAsync(nameof(Signin));
    }
}