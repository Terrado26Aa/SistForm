using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class EditProfilePage : ContentPage
{
    private int userId;
    private readonly ApiService _apiService;

    public EditProfilePage()
	{
		InitializeComponent();
        _apiService = new ApiService();
	}

	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserData();
    }

    private async Task LoadUserData()
    {
        try
        {
            string userIdString = DeviceInfo.Platform == DevicePlatform.MacCatalyst 
                ? Preferences.Default.Get("user_id", "") 
                : await SecureStorage.Default.GetAsync("user_id") ?? "";
            if (int.TryParse(userIdString, out userId))
            {
                var result = await _apiService.GetUserProfileAsync(userId);

                if (result.IsSuccess && result.Value != null)
                {
                    FirstNameEntry.Text = result.Value.FirstName;
                    LastNameEntry.Text = result.Value.LastName;
                    EmailEntry.Text = result.Value.Email;
                    RoleLabel.Text = $"Rol: {result.Value.Role ?? "User"}";
                }
                else
                {
                    await DisplayAlert("Error", result.ErrorMessage ?? "No se pudieron cargar tus datos.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo identificar al usuario.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", $"No se pudo conectar con el servidor: {ex.Message}", "OK");
        }
    }

    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) || string.IsNullOrWhiteSpace(LastNameEntry.Text) || 
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
            return;
        }

        string? passwordToSend = null;

        if (!string.IsNullOrWhiteSpace(NewPasswordEntry.Text) || !string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }
            passwordToSend = NewPasswordEntry.Text;
        }

        var updatedProfile = new UserProfileDto
        {
            FirstName = FirstNameEntry.Text,
            LastName = LastNameEntry.Text,
            Email = EmailEntry.Text,
            NewPassword = passwordToSend
        };

        var result = await _apiService.UpdateUserProfileAsync(userId, updatedProfile);

        if (result.IsSuccess)
        {
            await DisplayAlert("Éxito", "Tu perfil ha sido actualizado.", "OK");
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage ?? "Error desconocido", "OK");
        }
    }
}
