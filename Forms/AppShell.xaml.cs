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
        }
    }
}
