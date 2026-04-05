using Forms.Services;
using Forms.Models;

namespace Forms.Views;

public partial class Signin : ContentPage
{
    private readonly ApiService _apiService;
	public Signin()
	{
		InitializeComponent();
        _apiService = new ApiService();
	}

	private async void OnRegisterButtonClicked(object sender, EventArgs e)
	{
        ErrorLabel.IsVisible = false;

        //Validaci�n del lado del cliente.
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
            string.IsNullOrWhiteSpace(FirstnameEntry.Text) ||
            string.IsNullOrWhiteSpace(LastnameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            ErrorLabel.Text = "Por favor, completa todos los campos.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            ErrorLabel.Text = "Las contrase�as no coinciden.";
            ErrorLabel.IsVisible = true;
            return;
        }

        var registerData = new RegisterRequestDto
        {
            Username = UsernameEntry.Text,
            Firstname = FirstnameEntry.Text,
            Lastname = LastnameEntry.Text,
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };

        try
        {
            await _apiService.RegisterAsync(registerData);

            //Manejo de registro exitoso.
            await DisplayAlert("Exito", "Registro exitoso. Ahora puedes iniciar sesión.", "OK");

            //Redirigir el usuario al login.
            await Shell.Current.GoToAsync($"//{nameof(Login)}");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Error inesperado: {ex.Message}";
            ErrorLabel.IsVisible = true;
            return;
        }
    }
}