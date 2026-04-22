using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class SyncSurveysPage : ContentPage
{
	public SyncSurveysPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadPendingResponses();
	}

	private async Task LoadPendingResponses()
	{
		var pending = await LocalDatabaseHelper.GetPendingResponsesAsync();

		if (pending == null || !pending.Any())
		{
			PendingResponsesList.IsVisible = false;
			EmptyMessageLabel.IsVisible = true;
		}
		else
		{
			PendingResponsesList.IsVisible = true;
			EmptyMessageLabel.IsVisible = false;
			PendingResponsesList.ItemsSource = pending;
		}
	}

	private async void OnUploadClicked(object sender, EventArgs e)
	{
		var button = sender as ImageButton;
		var responseToSync = button?.CommandParameter as SubmitResponseDto;

		if (responseToSync == null) return;

		if(Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			await DisplayAlert("Sin Conexión", "Necesitas internet para subir la encuesta", "OK");
			return;
		}

		//Mostar indicador de carga
		button.IsEnabled = false;

		try
		{
			var api = new ApiService();

			bool isSuccess = await api.SubmitResponseAsync(responseToSync);

			if(isSuccess)
			{
				await LocalDatabaseHelper.DeletePendingResponseAsync(responseToSync.LocalId);
				await DisplayAlert("Éxito", "Encuesta subida correctamente", "OK");
                await LoadPendingResponses(); //Recargar la lista para reflejar los cambios
            }
            else
            {
                await DisplayAlert("Error", "El servidor rechazo los datos. Intenta denuevo", "OK");
            }
        }
		catch (Exception ex)
        {
			await DisplayAlert("Error", $"Ocurrió un error al subir la encuesta: {ex.Message}", "OK");
        }
		finally
		{
            button.IsEnabled = true;
        }
	}

	private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var responseToDelete = button?.CommandParameter as SubmitResponseDto;

        if (responseToDelete == null) return;

        bool confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que quieres eliminar esta encuesta pendiente?", "Sí", "No");

        if (confirm)
        {
            await LocalDatabaseHelper.DeletePendingResponseAsync(responseToDelete.LocalId);
            await LoadPendingResponses(); //Recargar la lista para reflejar los cambios
        }
    }

	private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var responseToEdit = button?.CommandParameter as SubmitResponseDto;

        if (responseToEdit == null) return;

		//Navegar a la página de edición, pasando la respuesta a editar
		await Navigation.PushAsync(new FillSurveyPage(responseToEdit.FormId, responseToEdit.LocalId));
	}
}