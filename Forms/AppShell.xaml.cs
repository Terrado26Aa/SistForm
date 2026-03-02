using Forms.Views;

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
        }

        // Verifica el estado de login cuando la pagina aparece
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Solo verifica el estado de login una vez
            if (_hasCheckedLogin) return;

            _hasCheckedLogin = true;

            // Busca el token
            string token = await SecureStorage.Default.GetAsync("auth_token");

            //borrar despues
            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Debug", "No hay token. Debería ir al Login.", "OK");
                await Shell.Current.GoToAsync("//Login");
            }
            else
            {
                await DisplayAlert("Debug", "¡Token encontrado! Bienvenido de nuevo.", "OK");
                // No hace nada, se queda en el HomePage que es la página por defecto
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await Current.DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");

            if (confirm)
            {
                // Borra el token guardado
                SecureStorage.Default.Remove("auth_token");

                // Navegar a la página de login y limpiar el historial de navegación
                await Shell.Current.GoToAsync($"//{nameof(Views.Login)}");
            }
        }
    }
}
