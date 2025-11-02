using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Layouts;
namespace Forms.Views;

public partial class CreateForm : ContentPage
{
    //variable para guardar el elemento seleccionado
    private View _selectedElement;
    //variable para guardar las dimensiones iniciales del elemento
    private Rect _startBounds;

    public CreateForm()
	{
		InitializeComponent();
    }

    private void OnElementPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var element = (View)sender;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                // Guarda las dimensiones iniciales del elemento cuando comienza el gesto
                _startBounds = AbsoluteLayout.GetLayoutBounds(element);
                break;

            case GestureStatus.Running:
                // Actualiza la posición del elemento mientras se arrastra
                var newX = _startBounds.X + e.TotalX;
                var newY = _startBounds.Y + e.TotalY;

                // Asegura que el elemento no se mueva fuera del lienzo
                newX = Math.Max(0, newX);
                newY = Math.Max(0, newY);

                AbsoluteLayout.SetLayoutBounds(element, new Rect(newX, newY, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                //no hacemos nada en estos casos
                break;
        }
    }

    private async void OnElementTapped(object sender, TappedEventArgs e)
    {
        _selectedElement = sender as View; //Guarda el elemento seleccionado
        if (_selectedElement == null) return;

        //Limpia cualquier selección visual anterior
        foreach (var child in CanvasLayout.Children.OfType<View>())
        {
            child.Scale = 1; //Restaura la escala
        }

        //Resalta visualmente el elemento seleccionado
        _selectedElement.Scale = 1.1; // Lo hace un poco mas grande

        // crea una instancia del popup de la toolbar de propiedades
        var toolbarPopup = new PropertiesToolBarPopup(_selectedElement);

        //Ancla el popup al elemento seleccionado
        toolbarPopup.Anchor = _selectedElement;

        // Muestra el popup
        await this.ShowPopupAsync(toolbarPopup);
    }

    private void AddDraggableElement(View element, string elementType)
	{
        //posición inicial del elemento
        AbsoluteLayout.SetLayoutBounds(element, new Rect(50, 50, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.PositionProportional);

        //Gesto de toque para seleccionar el elemento y mostrar el panel
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnElementPanUpdated;
        element.GestureRecognizers.Add(panGesture);

        //Gesto de toque para seleccionar el elemento y mostrar el panel
        var tapGesture = new TapGestureRecognizer();
        //Conectamos el evento Tapped al manejador OnElementTapped
        tapGesture.Tapped += OnElementTapped;
        //Anadimos el gesto de toque al elemento
        element.GestureRecognizers.Add(tapGesture);

        //Agregamos el elemento al lienzo
        CanvasLayout.Children.Add(element);
    }

    private void OnAddTextClicked(object sender, EventArgs e)
    {
        //crea un nuevo Label
        var newLabel = new Label
        {
            Text = "Nuevo Texto",
            Padding = 5,
            FontSize = 16,
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightYellow,
        };

        //llama al metodo para agregar el elemento al lienzo
        AddDraggableElement(newLabel, "Texto");
    }

    private void OnAddImageClicked(object sender, EventArgs e)
    {
        //crea una nueva imagen
        var newImage = new Image
        {
            Source = "dotnet_bot", //Imagen de ejemplo
            WidthRequest = 100,
            HeightRequest = 100,
        };
        //llama al metodo para agregar el elemento al lienzo
        AddDraggableElement(newImage, "Imagen");
    }

    private void OnAddEntryClicked(object sender, EventArgs e)
    {
        //crea un nuevo Entry
        var newEntry = new Entry
        {
            Placeholder = "Ingrese texto",
            WidthRequest = 200,
            BackgroundColor = Colors.LightYellow,
        };
        //llama al metodo para agregar el elemento al lienzo
        AddDraggableElement(newEntry, "Campo de Entrada");
    }
}