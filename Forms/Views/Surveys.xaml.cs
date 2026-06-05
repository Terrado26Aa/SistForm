using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class Surveys : ContentPage
{
	public Surveys()
	{
		InitializeComponent();
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
            await DisplayAlert("Error", result.ErrorMessage, "OK");
        }
    }

    private async void OnFrameTapped(object sender, TappedEventArgs e)
    {
        var selectedForm = e.Parameter as FormDto; 
        if (selectedForm == null) return;
        await Navigation.PushAsync(new FillSurveyPage(selectedForm.IdForm)); 
    }

    private async void OnDownloadFormClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var selectedForm = button?.CommandParameter as FormDto;

        if (selectedForm == null) return;
        
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "No hay conexión a internet. Por favor, conéctate para descargar el formulario.", "OK");
            return;
        }

        var api = new ApiService();
        var result = await api.GetFormDetailsAsync(selectedForm.IdForm);

        if (result.IsSuccess && result.Value != null)
        {
            await LocalDatabaseHelper.SaveFormLocallyAsync(result.Value);
            await DisplayAlert("Exito", $"La encuesta '{result.Value.Title}' ha sido descargada y ya esta disponible offline", "OK");
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage ?? "No se pudo descargar la encuesta.", "OK");
        }
    }
}
