using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls;
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
    
    public ObservableCollection<ToolbarItem> ToolbarItems { get; set; }

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

    private void AddElementToNewRow(View newElement)
    {
        var rowDefinition = new RowDefinition { Height = GridLength.Auto };
        CanvasGrid.RowDefinitions.Add(rowDefinition);

        Grid.SetRow(newElement, _nextRow);

        CanvasGrid.Children.Add(newElement);

        _nextRow++;
    }

    //metodo para crear un frame que envuelve el elemento
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

        var editItem = new MenuFlyoutItem //accion de editar
        {
            Text = "Editar",
            IconImageSource = "edit_icon.png"
        };
        editItem.Command = new Command((param) =>
        {
            if (param is Border elementToEdit)
            {
                OnEditElement(elementToEdit);
            }
        });
        editItem.CommandParameter = borderElement; // pasa el border como parametro para editarlo

        var deleteItem = new MenuFlyoutItem //accion de eliminar
        {
            Text = "Eliminar",
            IconImageSource = "delete_icon.png"
        };
        deleteItem.Command = new Command((param) =>
        {
            if(param is View elementToDelete)
            {
                CanvasGrid.Children.Remove(elementToDelete);
            }
        });
        deleteItem.CommandParameter = borderElement; // pasa el border como parametro para eliminarlo

        // crea el menu contextual
        var menuFlyout = new MenuFlyout { editItem, deleteItem };
        MenuFlyout.SetContextFlyout(borderElement, menuFlyout);

        // agrega el frame al canvas en una nueva fila
        AddElementToNewRow(borderElement);
    }

    private void DeselectElement()
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

    private async void OnEditElement(Border selectedBorder)
    {
        _selectedElement = selectedBorder;

        DeselectElement();
        selectedBorder.Scale = 1.05;
        selectedBorder.Stroke = Colors.Blue;

        await DisplayAlert("Editar Elemento", "La funcionalidad de Editar se implementara pronto!", "OK");
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