namespace Forms.Views;

public partial class PropertiesPanelView : ContentView
{
    // Necesitamos una forma de saber qué elemento estamos editando.
    // Una forma simple es usar un evento.
    // Un patrón más avanzado usaría data binding o un ViewModel.

    //Propiedad publica para acceder al elemento seleccionado
    public View SelectedElement { get; set; }
    public PropertiesPanelView()
	{
		InitializeComponent();
	}

    //Cambiar tamaño de fuente
    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        if(SelectedElement is Label SelectedLabel && sender is Picker picker)
        {
            if(double .TryParse(picker.SelectedItem as string, out double newSize))
            {
                SelectedLabel.FontSize = newSize;
            }
        }
        //Añadir logica para otros tipos de elementos.(Buttom, entry, etc.)
    }

    //Cambiar la familia de fuente
    private void OnFamilyChanged(object sender, EventArgs e)
    {
        if(SelectedElement is Label SelectedLabel && sender is Picker picker)
        {
            SelectedLabel.FontFamily = picker.SelectedItem as string;
        }
    }

    //Alinear a la izquierda
    private void OnAlignLeftClicked(object sender, EventArgs e)
    {

    }
}