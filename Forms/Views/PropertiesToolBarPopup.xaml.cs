using CommunityToolkit.Maui.Views;

namespace Forms.Views;

public partial class PropertiesToolBarPopup : Popup
{
    //propiedad para guardar elementos que estamos editando
	private readonly View _selectedElement;
    //Constructor que recibe el elemento seleccionado
    public PropertiesToolBarPopup(View selectedElement)
	{
		InitializeComponent();
        _selectedElement = selectedElement;
    }

    //Manejador para la opcion de "Negrita"
    private async void OnBoldTapped(object sender, TappedEventArgs e)
    {
        if (_selectedElement is Label label)
        {
            label.FontAttributes = label.FontAttributes == FontAttributes.Bold 
                ? FontAttributes.None 
                : FontAttributes.Bold;
        }
        // Cerrar el popup despues de aplicar el cambio
        await CloseAsync();

    }

    //Manejador para la opcion de "Italica"
    private async void OnItalicTapped(object sender, TappedEventArgs e)
    {
        if (_selectedElement is Label label)
        {
            label.FontAttributes = label.FontAttributes == FontAttributes.Italic
                ? FontAttributes.None
                : FontAttributes.Italic;
        }
        // Cerrar el popup despues de aplicar el cambio
        await CloseAsync();
    }

    //Manejador para la opcion de "Subrayado"
    private async void OnUnderlineTapped(object sender, TappedEventArgs e)
    {
        if (_selectedElement is Label label)
        {
            label.TextDecorations = label.TextDecorations == TextDecorations.Underline
                ? TextDecorations.None
                : TextDecorations.Underline;
        }
        // Cerrar el popup despues de aplicar el cambio
        await CloseAsync();
    }
}