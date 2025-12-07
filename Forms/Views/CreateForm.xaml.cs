using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Forms.Models;

namespace Forms.Views;

public partial class CreateForm : ContentPage
{
    //variable para guardar el elemento seleccionado
    private View _selectedElement;
    //variable para guardar las dimensiones iniciales del elemento
    private int _nextRow = 0;
    
    public ObservableCollection<Forms.Models.ToolbarItem> ToolbarItems { get; set; }

    private ICommand _editCommand;
    private ICommand _deleteCommand;
    private ICommand _selectModeCommand;

    private bool _isSelectionMode;
    public bool IsSelectionMode
    {
        get => _isSelectionMode;
        set
        {
            _isSelectionMode = value;
            OnPropertyChanged(nameof(IsSelectionMode));
            //actualiza la visibilidad de los botones en la toolbar
            UpdateToolbarItemsVisibility();
        }
    }

    public CreateForm()
    {
        InitializeComponent();

        // Inicializa los comandos
        InitializeCommands();

        //creamos la lista de datos para el CarouselView de la toolbar
        ToolbarItems = new ObservableCollection<Forms.Models.ToolbarItem>
        {
            new Forms.Models.ToolbarItem{ Text = "Texto", IconImageSource = "text_icon.png", Command = new Command(OnAddTextClicked)},
            new Forms.Models.ToolbarItem{ Text = "Imagen", IconImageSource = "image_icon.png", Command = new Command(OnAddImageClicked) },
            new Forms.Models.ToolbarItem{ Text = "Checklist",IconImageSource = "checklist_icon.png", Command = new Command(OnAddChecklistClicked) },
            new Forms.Models.ToolbarItem{ Text = "Multiple", IconImageSource = "multiple_icon.png", Command = new Command(OnAddMultipleChoiceClicked) },

            // Añade los botones Contextuales
            new Forms.Models.ToolbarItem{ Text = "Seleccionar", IconImageSource = "select_icon.png", Command = _selectModeCommand },
            new Forms.Models.ToolbarItem{ Text = "Editar", IconImageSource = "edit_icon.png", Command = _editCommand, IsContextItem = true, IsVisible = false },
            new Forms.Models.ToolbarItem{ Text = "Eliminar", IconImageSource = "delete_icon.png", Command = _deleteCommand, IsContextItem = true, IsVisible = false },
        };

        this.BindingContext = this;
    }

    //Metodo de Inicializacion de Comandos
    private void InitializeCommands()
    {
        _selectModeCommand = new Command(ToggleSelectionMode);
        _editCommand = new Command(OnEditElement, CanExecuteContextAction);
        _deleteCommand = new Command(OnDeleteElement, CanExecuteContextAction);
    }

    // Metodo para actualizar la visibilidad de los items de la toolbar
    private void UpdateToolbarItemsVisibility()
    {
        // Recorre todos los items de la barra de herramientas
        foreach (var item in ToolbarItems)
        {
            if (item.IsContextItem)
            {
                //Muestra botones contextuales solo cuando esta en modo seleccion
                item.IsVisible = IsSelectionMode;
            }
            else
            {
                //Muestra botones normales solo cuando no esta en modo seleccion
                { item.IsVisible = !IsSelectionMode; }
            }
        }

        // Fuerza la actualizacion del CarouselView
        var temp = new ObservableCollection<Forms.Models.ToolbarItem>(ToolbarItems);
        ToolbarItems.Clear();
        foreach (var item in temp)
        {
            ToolbarItems.Add(item);
        }
    }

    // Logica para alternar el modo de seleccion
    private void ToggleSelectionMode()
    {
        IsSelectionMode = !IsSelectionMode;

        if (!IsSelectionMode)
        {
            DeselectAllElement();
            _selectedElement = null;
        }

        var selectButton = ToolbarItems.FirstOrDefault(item => item.Command == _selectModeCommand);
        if (selectButton != null)
        {
            selectButton.Text = IsSelectionMode ? "Cancelar" : "Seleccionar";
            selectButton.IconImageSource = IsSelectionMode ? "cancel_icon.png" : "select_icon.png";
        }
    }

    // Logica para editar el elemento seleccionado
    private async void OnEditElement()
    {
        if (_selectedElement == null) return;

        await DisplayAlert("Editar Elemento", "La funcionalidad de Editar se implementara pronto!", "OK");
    }

    // Logica para eliminar el elemento seleccionado
    private async void OnDeleteElement()
    {
        if (_selectedElement == null)
        {
            CanvasGrid.Children.Remove(_selectedElement);
            _selectedElement = null;

            // Refresca el estado de los comandos
            ((Command)_deleteCommand).ChangeCanExecute();
            ((Command)_editCommand).ChangeCanExecute();
        }
    }

    // Este metodo decide si los comandos contextuales pueden ejecutarse
    private bool CanExecuteContextAction()
    {
        //solo permite ejecutar si hay un elemento seleccionado
        return _selectedElement != null;
    }

    // Logica para deseleccionar todos los elementos
    private void DeselectAllElement()
    {
        foreach (var child in CanvasGrid.Children)
        {
            if (child is Border border)
            {
                border.Scale = 1.0;
                border.Stroke = Colors.LightGray;
            }
        }
    }

    // Manejador del evento Tapped para los elementos
    private void OnElementTapped(object sender, TappedEventArgs e)
    {
        if (!IsSelectionMode) return; // solo permite seleccionar en modo seleccion

        var tappedBorder = sender as Border;
        if (tappedBorder == null) return;

        DeselectAllElement();

        _selectedElement = tappedBorder; // guarda el elemento seleccionado

        tappedBorder.Scale = 1.05;
        tappedBorder.Stroke = Colors.Blue;

        // Refresca el estado de los comandos
        ((Command)_deleteCommand).ChangeCanExecute();
        ((Command)_editCommand).ChangeCanExecute();
    }

    //Metodo para agregar un nuevo elemento en una nueva fila del grid
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

    //Metodo para crear un Border que envuelve el elemento
    private Border CreateBorderElement(View element)
    {
        return new Border
        {
            Content = element,
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            Background = Colors.White,
            Margin = new Thickness(5),
            Padding = new Thickness(15, 10),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(8),
            },
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Opacity = 0.3f,
                Radius = 5,
                Offset = new Point(2, 2),
            },
        };
    }

    //Metodo para crear y agregar un nuevo elemento al grid
    private void CreateAndAddElement(View mainContent)
    {
        //crea un nuevo Entry para el titulo
        var titleInput = new Entry
        {
            Placeholder = "Ingrese el titulo",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black,
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

        // agrega el titulo y el contenido principal al grid
        innerGrid.Add(titleInput, 0, 0);
        Grid.SetColumnSpan(mainContent, 2); // hace que el contenido principal ocupe ambas columnas
        innerGrid.Add(mainContent, row: 1, column: 0);

        //envuelve el grid en un border
        var borderElement = CreateBorderElement(innerGrid);

        // agrega el frame al canvas en una nueva fila
        AddElementToNewRow(borderElement);
    }

    // Metodo para agregar un elemento de Texto
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

    // Metodo para agregar un elemento de Imagen
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

    // Metodo para agregar un elemento de Checklist
    private void OnAddChecklistClicked()
    {
        //crea un contenedor vertical para la checklist
        var checklistStack = new VerticalStackLayout { Spacing = 5 };

        //crea el boton "Agregar ?tem" que ira dentro del checklist
        var addItemButton = new Button
        {
            Text = "Agregar ?tem",
            WidthRequest = 100,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.CornflowerBlue,
            HorizontalOptions = LayoutOptions.Start,
        };

        //define la accion del bot?n "agregar item" usando la funcion lambda.
        addItemButton.Clicked += (sender, e) =>
        {
            //crea un nuevo item para la checklist (checkbox + entry)
            var newItemLayout = new HorizontalStackLayout { Spacing = 5 };
            newItemLayout.Children.Add(new CheckBox());
            newItemLayout.Children.Add(new Entry
            {
                Placeholder = "Nuevo item",
                FontSize = 16,
                TextColor = Colors.Black,
                VerticalOptions = LayoutOptions.Center,
            });

            //inserta el nuevo ?tem antes del boton "Agregar item"
            checklistStack.Children.Insert(checklistStack.Children.Count - 1, newItemLayout);
        };

        // crea el primer item de la checklist
        var firstItemLayout = new HorizontalStackLayout { Spacing = 5 };
        firstItemLayout.Children.Add(new CheckBox());
        firstItemLayout.Children.Add(new Entry
        {
            Placeholder = "Nuevo item",
            FontSize = 16,
            TextColor = Colors.Black,
            VerticalOptions = LayoutOptions.Center,
        });

        // agrega el checklistStack (con el primer item y el boton) al canvas
        checklistStack.Children.Add(firstItemLayout);
        checklistStack.Children.Add(addItemButton);

        CreateAndAddElement(checklistStack);
    }

    // Manejador del evento CheckedChanged para las casillas de multiple choice
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

            await DisplayAlert("Limite alcanzado", $"Solo puede seleccionar hasta {limit} opciones.", "OK");
        }
    }

    // Metodo para agregar un elemento de Multiple Choice
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
            Placeholder = "Numero maximo de selecciones",
            Keyboard = Keyboard.Numeric,
            WidthRequest = 50,
        };

        // crea el boton para agregar opciones que ira dentro del multiple choice
        var addOptionButton = new Button
        {
            Text = "Agregar opcion",
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
            newOptionLayout.Children.Add(new Entry { Placeholder = "Nueva opcion", VerticalOptions = LayoutOptions.Center, });

            // inserta la nueva opcion antes del bot?n "Agregar opcion"
            multipleChoiceLayout.Children.Insert(multipleChoiceLayout.Children.Count - 1, newOptionLayout);
        };

        var firstOptionLayout = new HorizontalStackLayout { Spacing = 5 };
        var firstCheckbox = new CheckBox();
        firstCheckbox.CheckedChanged += OnCheckBoxCheckedChanged; // asigna el manejador de evento para controlar el limite de selecciones

        firstOptionLayout.Children.Add(firstCheckbox);
        firstOptionLayout.Children.Add(new Entry { Placeholder = "Nueva opcion", VerticalOptions = LayoutOptions.Center, });

        //añade todos los componentes al contenedor principal en orden.
        multipleChoiceLayout.Children.Add(limitLable);
        multipleChoiceLayout.Children.Add(limitEntry);
        multipleChoiceLayout.Children.Add(firstOptionLayout);
        multipleChoiceLayout.Children.Add(addOptionButton);

        CreateAndAddElement(multipleChoiceLayout);
    }
}