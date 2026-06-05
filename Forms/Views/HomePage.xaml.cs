using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class HomePage : ContentPage
{
    private ApiService _apiService;

    public HomePage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDashboardData();
        CheckUserRole();
    }

    private async void CheckUserRole()
    {
        string role = DeviceInfo.Platform == DevicePlatform.MacCatalyst 
            ? Preferences.Default.Get("user_role", "User") 
            : await SecureStorage.Default.GetAsync("user_role") ?? "User";
        
        bool isAdmin = role == "Admin";
        
        CreateSurveyButton.IsVisible = isAdmin;
        ManageUsersButton.IsVisible = isAdmin;
        
        WelcomeLabel.Text = $"Bienvenido {(isAdmin ? "Administrador" : "Usuario")}";
    }

    private async Task LoadDashboardData()
    {
        try
        {
            var result = await _apiService.GetDashboardStatsAsync();
            if (result.IsSuccess && result.Value != null)
            {
                var stats = result.Value;
                TotalFormsLabel.Text = stats.TotalForms.ToString();
                TotalUsersLabel.Text = stats.TotalUsers.ToString();
                TotalResponsesLabel.Text = stats.TotalResponses.ToString();
                RecentFormsList.ItemsSource = stats.RecentForms;
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage ?? "No se pudieron cargar las estadísticas", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", $"No se pudo conectar con el servidor: {ex.Message}", "OK");
        }
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CreateForm");
    }

    private async void OnViewAllClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ManageFormsPage");
    }

    private async void OnManageUsersClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//UserManagementPage");
    }
}
