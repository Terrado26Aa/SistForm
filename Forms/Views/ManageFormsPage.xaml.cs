using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class ManageFormsPage : ContentPage
{
    public bool IsAdmin { get; set; }

    // Formulario actualmente seleccionado para asignar
    private FormDto? _formToAssign;

    public ManageFormsPage()
    {
        InitializeComponent();
        CheckAdmin();
        BindingContext = this;
    }

    private async void CheckAdmin()
    {
        var role = DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? Preferences.Default.Get("user_role", "User")
            : await SecureStorage.Default.GetAsync("user_role") ?? "User";
        IsAdmin = role == "Admin";
        OnPropertyChanged(nameof(IsAdmin));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadForms();
    }

    private async Task LoadForms()
    {
        var api = new ApiService();
        var result = await api.GetAllFormsAsync();
        if (result.IsSuccess)
        {
            FormsList.ItemsSource = result.Value;
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage ?? "Error desconocido", "OK");
        }
    }

    // ─── Botón Asignar: abre el popup con la lista de usuarios ───────────────
    private async void OnAssignClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        _formToAssign = button?.CommandParameter as FormDto;
        if (_formToAssign == null) return;

        // Título del popup
        PopupFormTitleLabel.Text = $"Encuesta: {_formToAssign.Title}";

        // Resetear estado del popup
        PopupUsersList.ItemsSource = null;
        PopupUsersList.SelectedItem = null;
        ConfirmAssignButton.IsEnabled = false;

        // Mostrar popup con animación
        AssignPopupOverlay.IsVisible = true;
        AssignPopupOverlay.Opacity = 0;
        await AssignPopupOverlay.FadeTo(1, 200);

        // Cargar lista de usuarios
        await LoadUsersForPopup();
    }

    private async Task LoadUsersForPopup()
    {
        PopupLoadingIndicator.IsRunning = true;
        PopupLoadingIndicator.IsVisible = true;

        var api = new ApiService();
        var result = await api.GetUsersAsync();

        PopupLoadingIndicator.IsRunning = false;
        PopupLoadingIndicator.IsVisible = false;

        if (result.IsSuccess)
        {
            PopupUsersList.ItemsSource = result.Value;
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage ?? "No se pudo cargar la lista de usuarios.", "OK");
            await ClosePopupAsync();
        }
    }

    // Habilita el botón Confirmar cuando se selecciona un usuario
    private void OnUserSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ConfirmAssignButton.IsEnabled = e.CurrentSelection?.Count > 0;
    }

    // ─── Confirmar la asignación ──────────────────────────────────────────────
    private async void OnConfirmAssignClicked(object sender, EventArgs e)
    {
        var selectedUser = PopupUsersList.SelectedItem as UserDto;
        if (selectedUser == null || _formToAssign == null) return;

        ConfirmAssignButton.IsEnabled = false;
        PopupLoadingIndicator.IsRunning = true;
        PopupLoadingIndicator.IsVisible = true;

        var api = new ApiService();
        var result = await api.AssignFormAsync(_formToAssign.IdForm, selectedUser.Id);

        PopupLoadingIndicator.IsRunning = false;
        PopupLoadingIndicator.IsVisible = false;
        ConfirmAssignButton.IsEnabled = true;

        if (result.IsSuccess)
        {
            await ClosePopupAsync();
            await DisplayAlert("✅ Asignada",
                $"La encuesta '{_formToAssign.Title}' ha sido asignada a {selectedUser.UserName} correctamente.",
                "OK");
        }
        else
        {
            // Mensaje amigable si ya estaba asignada
            string msg = result.ErrorMessage?.Contains("ya está asignada") == true
                ? $"'{_formToAssign.Title}' ya estaba asignada a {selectedUser.UserName}."
                : result.ErrorMessage ?? "No se pudo completar la asignación.";
            await DisplayAlert("Aviso", msg, "OK");
        }
    }

    // ─── Cerrar popup ─────────────────────────────────────────────────────────
    private async void OnClosePopupClicked(object sender, EventArgs e) => await ClosePopupAsync();
    private async void OnOverlayTapped(object sender, TappedEventArgs e) => await ClosePopupAsync();

    private async Task ClosePopupAsync()
    {
        await AssignPopupOverlay.FadeTo(0, 150);
        AssignPopupOverlay.IsVisible = false;
        _formToAssign = null;
    }

    // ─── Eliminar encuesta ────────────────────────────────────────────────────
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var formToDelete = button?.CommandParameter as FormDto;
        if (formToDelete == null) return;

        bool confirm = await DisplayAlert("Confirmar",
            $"¿Eliminar el formulario '{formToDelete.Title}'?\nSe eliminarán también todas sus respuestas.",
            "Eliminar", "Cancelar");

        if (confirm)
        {
            var api = new ApiService();
            var result = await api.DeleteFormAsync(formToDelete.IdForm);
            if (result.IsSuccess)
            {
                await DisplayAlert("Éxito", "Formulario eliminado correctamente.", "OK");
                await LoadForms();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage ?? "No se pudo eliminar el formulario.", "OK");
            }
        }
    }

    // ─── Editar encuesta ──────────────────────────────────────────────────────
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var formToEdit = button?.CommandParameter as FormDto;
        if (formToEdit == null) return;
        await Navigation.PushAsync(new EditFormPage(formToEdit.IdForm));
    }

    // ─── Ver resultados ───────────────────────────────────────────────────────
    private async void OnViewResultsClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var form = button?.CommandParameter as FormDto;
        if (form == null) return;
        await Navigation.PushAsync(new ResultsDashboardPage(form.IdForm, form.Title));
    }

    // ─── Crear nueva encuesta ─────────────────────────────────────────────────
    private async void OnCreateNewClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CreateForm");
    }
}
