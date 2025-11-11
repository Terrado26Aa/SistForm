namespace Forms.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Navega de regreso al login
        await Shell.Current.GoToAsync($"//{nameof(Login)}");
    }
}