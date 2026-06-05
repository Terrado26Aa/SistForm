using Forms.Views;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Forms
{
    public partial class AppShell : Shell
    {

        private bool _hasCheckedLogin = false; // Variable para evitar comprobaciones repetidas
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Signin), typeof(Signin));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(CreateForm), typeof(CreateForm));
            Routing.RegisterRoute(nameof(Surveys), typeof(Surveys));
            Routing.RegisterRoute(nameof(ManageFormsPage), typeof(ManageFormsPage));
            Routing.RegisterRoute(nameof(FillSurveyPage), typeof(FillSurveyPage));
            Routing.RegisterRoute(nameof(EditFormPage), typeof(EditFormPage));
            Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));
            Routing.RegisterRoute(nameof(SyncSurveysPage), typeof(SyncSurveysPage));
        }

        // Verifica el estado de login cuando la pagina aparece
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_hasCheckedLogin) return;
            _hasCheckedLogin = true;

            string token = DeviceInfo.Platform == DevicePlatform.MacCatalyst 
                ? Preferences.Default.Get("auth_token", "") 
                : await SecureStorage.Default.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token) && !IsTokenExpired(token))
            {
                string role = DeviceInfo.Platform == DevicePlatform.MacCatalyst 
                    ? Preferences.Default.Get("user_role", "User") 
                    : await SecureStorage.Default.GetAsync("user_role") ?? "User";
                
                ConfigureMenu(role, false);

                if (role == "Admin")
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                else
                    await Shell.Current.GoToAsync($"//{nameof(Surveys)}");
            }
            else if (!string.IsNullOrEmpty(token))
            {
                // El token expiró: limpiar sesión y quedarse en el Login
                await ForceLogoutAsync();
            }
        }

        public void ConfigureMenu(string role, bool isOffline)
        {
            MenuHomePage.IsVisible = !isOffline && role == "Admin";
            MenuCreateForm.IsVisible = !isOffline && role == "Admin";
            MenuSurveys.IsVisible = !isOffline;
            MenuOfflineSurveys.IsVisible = isOffline || role == "User";
            MenuManageForms.IsVisible = !isOffline && role == "Admin";
            MenuUserManagement.IsVisible = !isOffline && role == "Admin";
            MenuSyncSurveys.IsVisible = true;
        }

        // Detecta si un JWT está expirado sin necesitar la clave secreta
        private static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JsonWebTokenHandler();
                var jwt = handler.ReadJsonWebToken(token);
                return jwt.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true; // Si no puede leerlo, lo consideramos expirado
            }
        }

        // Llamado desde ApiService cuando detecta un 401 en cualquier petición
        public static async Task ForceLogoutAsync()
        {
            if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
            {
                Preferences.Default.Remove("auth_token");
                Preferences.Default.Remove("user_id");
                Preferences.Default.Remove("user_role");
            }
            else
            {
                SecureStorage.Default.Remove("auth_token");
                SecureStorage.Default.Remove("user_id");
                SecureStorage.Default.Remove("user_role");
            }

            // Notificar al usuario y redirigir al login
            if (Shell.Current != null)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert(
                        "Sesión Expirada",
                        "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.",
                        "OK");
                    await Shell.Current.GoToAsync($"//{nameof(Login)}");
                });
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await Current.DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");

            if (confirm)
            {
                if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                {
                    Preferences.Default.Remove("auth_token");
                    Preferences.Default.Remove("user_id");
                    Preferences.Default.Remove("user_role");
                }
                else
                {
                    SecureStorage.Default.Remove("auth_token");
                    SecureStorage.Default.Remove("user_id");
                    SecureStorage.Default.Remove("user_role");
                }

                _hasCheckedLogin = false;
                await Shell.Current.GoToAsync($"//{nameof(Views.Login)}");
            }
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            Current.FlyoutIsPresented = false; // Cierra el menú lateral

            await Shell.Current.GoToAsync(nameof(EditProfilePage));
        }
    }
}
