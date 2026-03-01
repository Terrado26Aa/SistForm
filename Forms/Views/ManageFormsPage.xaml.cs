using Forms.Models;
using Forms.Services;

namespace Forms.Views;

public partial class ManageFormsPage : ContentPage
{
	public ManageFormsPage()
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
        var forms = await api.GetAllFormsAsync();
        FormsList.ItemsSource = forms;
    }

    //acción para eliminar un formulario
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        // Obtener el formulario asociado al botón de eliminar
        var button = sender as Button;
        var formToDelete = button?.CommandParameter as FormDto;

        if (formToDelete == null) return;

        // Mostrar una alerta de confirmación antes de eliminar
        bool confirm = await DisplayAlert("Confirmar", $"¿Estás seguro de que deseas eliminar el formulario '{formToDelete.Title}'?", "Eliminar", "Cancelar");

        // Si el usuario confirma, proceder a eliminar el formulario
        if (confirm) 
        {
            // Llamar al servicio en la API para eliminar el formulario
            var api = new ApiService();
            bool success = await api.DeleteFormAsync(formToDelete.IdForm);

            if (success)
            {
                await DisplayAlert("Éxito", "Formulario eliminado correctamente.", "OK");
                await LoadForms(); // Recargar la lista de formularios después de eliminar
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el formulario. Inténtalo de nuevo.", "OK");
            }
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        // Obtener el formulario asociado al botón de editar
        var button = sender as Button;
        var formToEdit = button?.CommandParameter as FormDto;

        if (formToEdit == null) return;

        // Navegar a la página de edición de formularios, pasando el formulario seleccionado
        await Navigation.PushAsync(new EditFormPage(formToEdit.IdForm));
    }
}