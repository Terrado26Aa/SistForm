using Forms.Models;
using Forms.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Forms.Views;

public partial class Login : ContentPage
{
    //Cliente HTTP para hacer las peticiones
    private readonly ApiService _apiService;

    public Login(ApiService apiService)
    {
        InitializeComponent();
        //Creamos una instancia del servicio
        _apiService = apiService;

    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        // Validaciones basicas
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Usuario y contrase?a no pueden estar vacios.");
            return;
        }

        // Creamos un objeto con los datos del login
        var loginRequest = new LoginRequestDto { UserName = username, Password = password };

        try
        {
            // Hacemos la peticion de login
            LoginResponseDto loginResponse = await _apiService.LoginAsync(loginRequest);

            if (loginResponse != null)
            {
                // Autenticaci?n exitosa
                await DisplayAlert("?xito", $"Login Correcto. {loginResponse.Message}", "OK");
                // navega hacia la pagina principal
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
        }
        catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ShowError("Usuario o contrase?a incorrectos. Por favor, intente denuevo.");
        }
        catch (HttpRequestException httpEx)
        {
            ShowError($"Error de conexion o del servidor: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            ShowError($"Error inesperado: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private async void OnSigninClicked(object sender, EventArgs e)
    {
        // Navega a la pagina de registro
        await Shell.Current.GoToAsync(nameof(Signin));
    }
}