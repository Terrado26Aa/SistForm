using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class Surveys : ContentPage
{
	public Surveys()
	{
		InitializeComponent();
	}

    //Cargar la lista de formularios cada vez que la página aparezca
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadForms();
    }

    private async Task LoadForms()
    {
        var api = new ApiService();
        //ActivityIndicator.IsRunning = true; // Mostrar el indicador de actividad mientras se cargan los datos
        var forms = await api.GetAllFormsAsync(); // Obtener la lista de formularios desde la API
        FormsList.ItemsSource = forms; // Asignar la lista al CollectionView
    }

    private async void OnFrameTapped(object sender, TappedEventArgs e)
    {
        // Obtener el formulario seleccionado a través del parámetro del evento
        var selectedForm = e.Parameter as FormDto; 

        if (selectedForm == null) return;

        // Navegar a la página de llenado de formulario, pasando el ID del formulario seleccionado
        await Navigation.PushAsync(new FillSurveyPage(selectedForm.IdForm)); 

    }

    private async void OnDownloadFormClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var selectedForm = button?.CommandParameter as FormDto;

        if (selectedForm == null) return;
        
        //Verifica si hay internet para poder descargar.
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "No hay conexión a internet. Por favor, conéctate para descargar el formulario.", "OK");
            return;
        }

        //Descargamos la encuesta Completa con todas sus preguntas desde la API.
        var api = new ApiService();
        var completeForm = await api.GetFormDetailsAsync(selectedForm.IdForm);

        if (completeForm != null)
        {
            //la guardamos en la memoria interna del telefono.
            await LocalDatabaseHelper.SaveFormLocallyAsync(completeForm);
            await DisplayAlert("Exito", $"La encuesta '{completeForm.Title}' ha sido descargada y ya esta disponible offline",
                "OK");
        }
        else
        {
            await DisplayAlert("Error", "No se pudo descargar la encuesta.", "OK");
        }
    }
}