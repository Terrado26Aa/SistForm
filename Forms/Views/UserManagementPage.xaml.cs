using Forms.Models;
using Forms.Services;

namespace Forms.Views
{
    public partial class UserManagementPage : ContentPage
    {
        private ApiService _apiService;

        public UserManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            var result = await _apiService.GetUsersAsync();
            if (result.IsSuccess)
            {
                UsersList.ItemsSource = result.Value;
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage ?? "Error desconocido", "OK");
            }
        }

        private async void OnCreateUserClicked(object sender, EventArgs e)
        {
            string username = await DisplayPromptAsync("Nuevo Usuario", "Nombre de usuario:");
            if (string.IsNullOrWhiteSpace(username)) return;

            string firstname = await DisplayPromptAsync("Nuevo Usuario", "Nombre:");
            string lastname = await DisplayPromptAsync("Nuevo Usuario", "Apellido:");
            string email = await DisplayPromptAsync("Nuevo Usuario", "Email:");
            string password = await DisplayPromptAsync("Nuevo Usuario", "Contraseña:", maxLength: 20, keyboard: Keyboard.Text);

            string roleChoice = await DisplayActionSheet("Seleccionar Rol", "Cancelar", null, "User", "Admin");
            string role = roleChoice == "Admin" ? "Admin" : "User";

            var newUser = new RegisterRequestDto
            {
                Username = username,
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                Password = password,
                Role = role
            };

            var result = await _apiService.RegisterAdminAsync(newUser);
            if (result.IsSuccess)
            {
                await DisplayAlert("Éxito", "Usuario creado exitosamente", "OK");
                await LoadUsers();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage ?? "Error desconocido", "OK");
            }
        }

        private async void OnEditRoleClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var user = button?.CommandParameter as UserDto;
            if (user == null) return;

            string newRole = await DisplayActionSheet($"Cambiar rol de {user.UserName}", "Cancelar", null, "User", "Admin");
            if (newRole != "User" && newRole != "Admin") return;

            var result = await _apiService.UpdateUserRoleAsync(user.Id, newRole);
            if (result.IsSuccess)
            {
                await DisplayAlert("Éxito", "Rol actualizado", "OK");
                await LoadUsers();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage ?? "Error desconocido", "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var user = button?.CommandParameter as UserDto;
            if (user == null) return;

            bool confirm = await DisplayAlert("Confirmar", $"¿Eliminar usuario {user.UserName}?", "Eliminar", "Cancelar");
            if (!confirm) return;

            var result = await _apiService.DeleteUserAsync(user.Id);
            if (result.IsSuccess)
            {
                await DisplayAlert("Éxito", "Usuario eliminado", "OK");
                await LoadUsers();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage ?? "Error desconocido", "OK");
            }
        }
    }
}
