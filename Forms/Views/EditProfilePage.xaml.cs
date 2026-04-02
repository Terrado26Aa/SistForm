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

        var updatedProfile = new UserProfileDto
        {
            FirstName = FirstNameEntry.Text,
            LastName = LastNameEntry.Text,
            Email = EmailEntry.Text
        };

        var api = new ApiService();
        bool success = await api.UpdateUserProfileAsync(userId, updatedProfile);

        if (success)
        {
            await DisplayAlert("Éxito", "Tu perfil ha sido actualizado.", "OK");
            await Shell.Current.GoToAsync(nameof(HomePage)); // Volver a la página principal.
        }
        else
        {
            await DisplayAlert("Error", "Hubo un problema al guardar los cambios.", "OK");
        }
    }
}