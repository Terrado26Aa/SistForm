using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class EditProfilePage : ContentPage
{
    private int userId;
    public EditProfilePage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserData();
    }

    private async Task LoadUserData()
    {
        // Obtener el ID del usuario actual desde el almacenamiento seguro
        string userIdString = await SecureStorage.Default.GetAsync("user_id");
        if (int.TryParse(userIdString, out userId))
        {
            var api = new ApiService();
            var profileDate = await api.GetUserProfileAsync(userId);

            if (profileDate != null)
            {
                // Cargar los datos en los campos de entrada
                FirstNameEntry.Text = profileDate.FirstName;
                LastNameEntry.Text = profileDate.LastName;
                EmailEntry.Text = profileDate.Email;
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar tus datos.", "OK");
            }
        }
    }
    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        //validaciones basicas
        if(string.IsNullOrWhiteSpace(FirstNameEntry.Text) || string.IsNullOrWhiteSpace(LastNameEntry.Text) || 
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
            return;
        }

        string passwordToSend = null;

        //validar si el usuario quiere cambiar su contraseña, si es asi, validar que las contraseñas coincidan
        if (!string.IsNullOrWhiteSpace(NewPasswordEntry.Text) || !string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }
            // Si las contraseñas coinciden, asignar la nueva contraseña a enviar
            passwordToSend = NewPasswordEntry.Text;
        }

        var updatedProfile = new UserProfileDto
        {
            FirstName = FirstNameEntry.Text,
            LastName = LastNameEntry.Text,
            Email = EmailEntry.Text,
            NewPassword = passwordToSend // Solo se enviará si el usuario ha ingresado una nueva contraseña
        };

        var api = new ApiService();
        var result = await api.UpdateUserProfileAsync(userId, updatedProfile);

        if (result.IsSuccess)
        {
            await DisplayAlert("Éxito", "Tu perfil ha sido actualizado.", "OK");
            await Shell.Current.GoToAsync("//HomePage"); // Volver a la página principal.
        }
        else
        {
            await DisplayAlert("Error de la API", result.ErrorMessage, "OK");
        }
    }
}