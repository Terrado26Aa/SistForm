using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Services;
using Forms.Views;
using System.Threading.Tasks;

namespace Forms.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private bool _isErrorVisible;

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Usuario y contraseña requeridos";
                IsErrorVisible = true;
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = "";
                IsErrorVisible = false;

                var api = new ApiService();
                var result = await api.LoginAsync(new Models.LoginRequestDto 
                { 
                    UserName = Username.Trim(), 
                    Password = Password.Trim() 
                });

                if (result.IsSuccess && result.Value != null)
                {
                    string role = result.Value.Role ?? "User";
                    if (Shell.Current is AppShell appShell)
                    {
                        appShell.ConfigureMenu(role, false);
                    }

                    if (role == "Admin")
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    else
                        await Shell.Current.GoToAsync($"//{nameof(Surveys)}");
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Error al iniciar sesión";
                    IsErrorVisible = true;
                    await Shell.Current.DisplayAlert("Error de Login", ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error inesperado: {ex.Message}";
                IsErrorVisible = true;
                await Shell.Current.DisplayAlert("Excepción", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(Signin));
        }

        [RelayCommand]
        public async Task GoToOfflineMode()
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.ConfigureMenu("User", true);
            }
            await Shell.Current.GoToAsync($"//{nameof(OfflineSurveysPage)}");
        }
    }
}
