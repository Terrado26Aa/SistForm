using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class OfflineSurveysPage : ContentPage
{
	public OfflineSurveysPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOfflineSurveys();
    }

    private async Task LoadOfflineSurveys()
    {
        var downloadedForms = await LocalDatabaseHelper.GetAllDownloadedFormAsync();

        if (downloadedForms == null || downloadedForms.Count == 0)
        {
            OfflineFormList.IsVisible = false;
            EmptyMessageLabel.IsVisible = true;
        }
        else
        {
            OfflineFormList.IsVisible = true;
            EmptyMessageLabel.IsVisible = false;
            OfflineFormList.ItemsSource = downloadedForms;
        }
    }

    private async void OnFillOfflineSurveyClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var selectedForm = button?.CommandParameter as FormDto;

        if (selectedForm != null)
            await Navigation.PushAsync(new FillSurveyPage(selectedForm.IdForm));
    }

    private async void OnDeleteOfflineFormClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var form = button?.CommandParameter as FormDto;

        if (form == null) return;

        bool confirm = await DisplayAlert(
            "Eliminar encuesta",
            $"¿Quieres eliminar '{form.Title}' del dispositivo?\nPodrás volver a descargarla cuando tengas conexión.",
            "Eliminar", "Cancelar");

        if (!confirm) return;

        await LocalDatabaseHelper.DeleteDownloadedFormAsync(form.IdForm);
        await LoadOfflineSurveys();
    }
}