using Forms.Models;

namespace Forms.Views;

public partial class Surveys : ContentPage
{
	public Surveys()
	{
		InitializeComponent();
	}

	private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedForm = e.CurrentSelection.FirstOrDefault() as FormDto;
		if (selectedForm != null) return;

        // Navegar a la página FillSurveyPage pasando el IdUser del formulario seleccionado
        await Navigation.PushAsync(new FillSurveyPage(selectedForm.IdUser));//verificar si es IdUser o IdForm.

        // Deseleccionar el ítem después de la navegación
        ((CollectionView)sender).SelectedItem = null;
    }
}