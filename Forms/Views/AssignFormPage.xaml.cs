using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class AssignFormPage : ContentPage
{
    private readonly int _formId;
    private readonly string _formTitle;

    public AssignFormPage(int formId, string formTitle)
    {
        InitializeComponent();
        _formId = formId;
        _formTitle = formTitle;
        FormTitleLabel.Text = $"Encuesta: {_formTitle}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        var api = new ApiService();
        var result = await api.GetUsersAsync();

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (result.IsSuccess)
        {
            UsersList.ItemsSource = result.Value;
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage, "OK");
        }
    }

    private async void OnAssignClicked(object sender, EventArgs e)
    {
        var selectedUser = UsersList.SelectedItem as UserDto;
        if (selectedUser == null)
        {
            await DisplayAlert("Atención", "Por favor selecciona un usuario de la lista.", "OK");
            return;
        }

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        var api = new ApiService();
        var result = await api.AssignFormAsync(_formId, selectedUser.Id);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (result.IsSuccess)
        {
            await DisplayAlert("Éxito", $"La encuesta ha sido asignada a {selectedUser.UserName}.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage, "OK");
        }
    }
}
