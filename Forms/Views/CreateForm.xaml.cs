using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Forms.Views;

public partial class CreateForm : ContentPage
{
    //variable para guardar el elemento seleccionado
    private View _selectedElement;
    //variable para guardar las dimensiones iniciales del elemento
    private int _nextRow = 0;

    public ObservableCollection<ToolbarItem> ToolbarItems {  get; set; }

    public CreateForm()
	{
		InitializeComponent();

        //inicializa los comandos para los botones de la toolbar
        var addTextCommand = new Command(OnAddTextClicked);
        var addImageCommand = new Command(OnAddImageClicked);
        var addChecklistCommand = new Command(OnAddChecklistClicked);
        var addMultipleChoiceCommand = new Command(OnAddMultipleChoiceClicked);

        //creamos la lista de datos para el CarouselView de la toolbar
        ToolbarItems = new ObservableCollection<ToolbarItem> 
        {
            new ToolbarItem{ Text = "Texto", IconImageSource = "text_icon.png", Command = addTextCommand},
            new ToolbarItem{ Text = "Imagen", IconImageSource = "image_icon.png", Command = addImageCommand },
            new ToolbarItem{ Text = "Checklist",IconImageSource = "checklist_icon.png", Command = addChecklistCommand },
            new ToolbarItem{ Text = "Multiple", IconImageSource = "multiple_icon.png", Command = addMultipleChoiceCommand },
        };

        this.BindingContext = this;
    }

    private async void OnElementTapped(object sender, TappedEventArgs e)
    {
        //Guarda el elemento seleccionado
        _selectedElement = sender as View;
        if (_selectedElement == null) return;

        foreach (var child in CanvasGrid.Children)
        {
            if (child is Grid container)
            {
                //obtiene el frame que envuelve el elemento
                var frame = container.Children.FirstOrDefault() as Frame;
                if (frame != null)
                {
                    frame.Scale = 1.0; // Restablece el tamaño original
                    frame.BorderColor = Colors.LightGray; // Restablece el color del borde
                }
            }
        }

        if (_selectedElement is Grid selectedContainer)
        {
            var frameToScale = selectedContainer.Children.FirstOrDefault() as Frame;
            if (frameToScale != null)
            {
                frameToScale.Scale = 1.5; // Aumenta el tamaño del frame
                frameToScale.BorderColor = Colors.Blue; // Cambia el color del borde para resaltar
            }
        }
    }

    private void AddElementToNewRow(View newElement)
    {
        var rowDefinition = new RowDefinition { Height = GridLength.Auto };
        CanvasGrid.RowDefinitions.Add(rowDefinition);

        Grid.SetRow(newElement, _nextRow);

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnElementTapped;
        newElement.GestureRecognizers.Add(tapGesture);

        CanvasGrid.Children.Add(newElement);

        _nextRow++;
    }

    //metodo para crear un frame que envuelve el elemento
    private Frame CreateFrameElement(View element)
    {
        return new Frame
        {
            Content = element,
            BorderColor = Colors.LightGray,
            CornerRadius = 8,
            HasShadow = true,
            Padding = new Thickness(15, 10),
            Margin = new Thickness(5),
            BackgroundColor = Colors.White,
        };
    }

    private void CreateAndAddElement (View mainContent)
    {
        //crea un nuevo Entry para el titulo
        var titleInput = new Entry
        {
            Placeholder = "Ingrese el titulo",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black,
        };

        //crea el boton eliminar
        var deleteButton = new ImageButton
        {
            Source = "delete_icon2.png",
            WidthRequest = 28,
            HeightRequest = 28,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.End,
            TranslationX = 10,
            TranslationY = -15,
        };

        //crea un grid interno para organizar el titulo y el contenido principal
        var innerGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
        };

        //envuelve el grid en un frame
        var frameElement = CreateFrameElement(innerGrid);

        // agrega el titulo y el contenido principal al grid
        innerGrid.Add(titleInput, 0, 0);
        Grid.SetColumnSpan(mainContent, 2); // hace que el contenido principal ocupe ambas columnas
        innerGrid.Add(mainContent, row: 1, column: 0);

        //define la accion del boton eliminar
        deleteButton.Clicked += OnDeleteButtonClicked;

        // crea un grid contenedor para el frame y el boton eliminar
        var containerGrid = new Grid();

        frameElement.ZIndex = 0; //el frame va detras
        deleteButton.ZIndex = 1; //el boton va delante

        containerGrid.Add(frameElement);
        containerGrid.Add(deleteButton);

        // agrega el frame al canvas en una nueva fila
        AddElementToNewRow(containerGrid);
    }

    private void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var deleteButton = sender as ImageButton;

        if (deleteButton?.Parent is Grid containerGrid)
        {
            //remueve el contenedor del grid principal
            CanvasGrid.Children.Remove(containerGrid);
        }
    }

    private void OnAddTextClicked()
    {
        //crea un nuevo Label
        var newLabel = new Label
        {
            Text = "Nuevo Texto",
            Padding = 5,
            FontSize = 16,
            TextColor = Colors.Black,
        };

        // agrega el label envuelto en un frame al grid
        CreateAndAddElement(newLabel);
    }

    private void OnAddImageClicked()
    {
        //crea un Boton para insertar la imagen
        var newButtom = new Button
        {
            Text = "Inserte una imagen",
            WidthRequest = 200,
            HeightRequest = 50,
        };

        CreateAndAddElement(newButtom);
    }

    private void OnAddChecklistClicked()
    {
        //crea un contenedor vertical para la checklist
        var checklistStack = new VerticalStackLayout { Spacing = 5 };

        //crea el botón "Agregar ítem" que ira dentro del checklist
        var addItemButton = new Button
        {
            Text = "Agregar ítem",
            WidthRequest = 100,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor= Colors.CornflowerBlue,
            HorizontalOptions = LayoutOptions.Start,
        };

        //define la acción del botón "agregar ítem" usando la función lambda.
        addItemButton.Clicked += (sender, e) => 
        {
            //crea un nuevo ítem para la checklist (checkbox + entry)
            var newItemLayout = new HorizontalStackLayout { Spacing = 5 };
            newItemLayout.Children.Add(new CheckBox());
            newItemLayout.Children.Add(new Entry
            {
                Placeholder = "Nuevo ítem",
                FontSize = 16,
                TextColor = Colors.Black,
                VerticalOptions = LayoutOptions.Center,
            });

            //inserta el nuevo ítem antes del botón "Agregar ítem"
            checklistStack.Children.Insert(checklistStack.Children.Count - 1, newItemLayout);
        };

        // crea el primer ítem de la checklist
        var firstItemLayout = new HorizontalStackLayout { Spacing = 5 };
        firstItemLayout.Children.Add(new CheckBox());
        firstItemLayout.Children.Add(new Entry
        {
            Placeholder = "Nuevo ítem",
            FontSize = 16,
            TextColor = Colors.Black,
            VerticalOptions = LayoutOptions.Center,
        });

        // agrega el checklistStack (con el primer ítem y el botón) al canvas
        checklistStack.Children.Add(firstItemLayout);
        checklistStack.Children.Add(addItemButton);

        CreateAndAddElement(checklistStack);
    }

    private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return; // si la casilla se desmarca, no hacer nada

        var changedCheckBox = sender as CheckBox;
        if (changedCheckBox == null) return;

        //buscamos el contenedor padre que tiene las opciones de multiple choice
        var parentLayout = changedCheckBox.Parent?.Parent as VerticalStackLayout;
        if (parentLayout == null) return;

        //obtenemos el Entry que define el limite de selecciones
        var limitEntry = parentLayout.Children.OfType<Entry>().FirstOrDefault();
        if (limitEntry == null) return;

        //obtenemos el limite de selecciones permitido
        int limit = int.TryParse(limitEntry.Text, out int parsedLimit) ? parsedLimit : int.MaxValue;

        if (limit <= 0) limit = int.MaxValue; // si el limite es 0 o negativo, no hay limite

        //contamos cuantas casillas estan marcadas actualmente
        int checkedCount = 0;
        foreach (var item in parentLayout.Children.OfType<HorizontalStackLayout>())
        {
            if (item.Children.OfType<CheckBox>().FirstOrDefault()?.IsChecked == true)
            {
                checkedCount++;
            }
        }

        if (checkedCount > limit)
        {
            Dispatcher.Dispatch(() =>
            {
                changedCheckBox.IsChecked = false; // desmarca la casilla que excede el limite
            });

            await DisplayAlert("Límite alcanzado", $"Solo puede seleccionar hasta {limit} opciones.", "OK");
        }
    }

    private void OnAddMultipleChoiceClicked()
    {
        // crea el contenedor para las opciones de multiple choice
        var multipleChoiceLayout = new VerticalStackLayout { Spacing = 8 };

        // crea los controles para agregar opciones
        var limitLable = new Label
        {
            Text = "Permitir seleccionar hasta:",
            FontSize = 14,
            FontAttributes = FontAttributes.Italic,
        };
        var limitEntry = new Entry
        {
            Placeholder = "Número máximo de selecciones",
            Keyboard = Keyboard.Numeric,
            WidthRequest = 50,
        };

        // crea el botón para agregar opciones que ira dentro del multiple choice
        var addOptionButton = new Button
        {
            Text = "Agregar opción",
            WidthRequest = 120,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.CornflowerBlue,
            HorizontalOptions = LayoutOptions.Start,
        };

        addOptionButton.Clicked += (s, args) =>
        {
            var newOptionLayout = new HorizontalStackLayout { Spacing = 5 };
            var newCheckbox = new CheckBox();

            //asigna el manejador de evento para controlar el limite de selecciones
            newCheckbox.CheckedChanged += OnCheckBoxCheckedChanged;

            newOptionLayout.Children.Add(newCheckbox);
            newOptionLayout.Children.Add(new Entry{ Placeholder = "Nueva opción", VerticalOptions = LayoutOptions.Center, });

            // inserta la nueva opción antes del botón "Agregar opción"
            multipleChoiceLayout.Children.Insert(multipleChoiceLayout.Children.Count - 1, newOptionLayout);
        };

        var firstOptionLayout = new HorizontalStackLayout { Spacing = 5 };
        var firstCheckbox = new CheckBox();
        firstCheckbox.CheckedChanged += OnCheckBoxCheckedChanged; // asigna el manejador de evento para controlar el limite de selecciones

        firstOptionLayout.Children.Add(firstCheckbox);
        firstOptionLayout.Children.Add(new Entry { Placeholder = "Nueva opción", VerticalOptions = LayoutOptions.Center, });

        //añade todos los componentes al contenedor principal en orden.
        multipleChoiceLayout.Children.Add(limitLable);
        multipleChoiceLayout.Children.Add(limitEntry);
        multipleChoiceLayout.Children.Add(firstOptionLayout);
        multipleChoiceLayout.Children.Add(addOptionButton);

        CreateAndAddElement(multipleChoiceLayout);
    }
}