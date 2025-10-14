namespace Forms
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.Login), typeof(Views.Login));
            Routing.RegisterRoute(nameof(Views.Signin), typeof(Views.Signin));
            Routing.RegisterRoute(nameof(Views.HomePage), typeof(Views.HomePage));
            Routing.RegisterRoute(nameof(Views.CreateForm), typeof(Views.CreateForm));
            Routing.RegisterRoute(nameof(Views.Surveys), typeof(Views.Surveys));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await Current.DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");

            if (confirm)
            {
                // Navegar a la página de login y limpiar el historial de navegación
                await Shell.Current.GoToAsync($"//{nameof(Views.Login)}");
            }
        }
    }
}
