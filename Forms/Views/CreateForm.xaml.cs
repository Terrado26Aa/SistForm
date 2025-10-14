using CommunityToolkit.Maui.Views;
namespace Forms.Views;

public partial class CreateForm : ContentPage
{
	private View _selectedElement; //variable para guardar el elemento seleccionado
	private readonly PropertiesPanelView _propertiesPanel; //Instancia del panel de propiedades
    public CreateForm()
	{
		InitializeComponent();
        _propertiesPanel = new PropertiesPanelView();
    }

	private void AddDraggableElement(View element, string elementType)
	{
        //(codigo para añadir el PanGestureRecorgnizer de arrastre)

        //Gesto de toque para seleccionar el elemento y mostrar el panel
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnElementTapped; //cambios de manejador
        element.GestureRecognizers.Add(tapGesture);

        CanvasLayout.Children.Add(element);
    }

    private async void OnElementTapped(object sender, TappedEventArgs e)
    {
        _selectedElement = sender as View; //Guarda el elemento seleccionado
        if (_selectedElement == null) return;
        
        //Limpia cualquier selección visual anterior
        foreach (var child in CanvasLayout.Children.OfType<View>())
        {
            child.Scale =1; //Restaura la escala
        }

        //Resalta visualmente el elemento seleccionado
        _selectedElement.Scale = 1.1; // Lo hace un poco mas grande

        _propertiesPanel.SelectedElement = _selectedElement;

        //Muestra el panel de propiedades como un BottomSheet
        await this.ShowBottomSheet(_propertiesPanel);
    }
}