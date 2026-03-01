using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Forms.Services;
using System.Text.RegularExpressions;

namespace Forms.Views;

public partial class EditFormPage : ContentPage
{
    //variable para guardar el elemento seleccionado
    private View _selectedElement;
    //variable para guardar las dimensiones iniciales del elemento
    private int _nextRow = 0;

    public ObservableCollection<ToolbarItem> ToolbarItems { get; set; }

    private int _formId;

    public EditFormPage(int formId)
    {
        InitializeComponent();
        _formId = formId;

        //inicializa los comandos para los botones de la toolbar
        var addTextCommand = new Command(OnAddTextClicked);
        var addImageCommand = new Command(OnAddImageClicked);
        var addChecklistCommand = new Command(OnAddChecklistClicked);
        var addMultipleChoiceCommand = new Command(OnAddMultipleChoiceClicked);
        var addSingleSelectionCommand = new Command(OnAddSingleSelectionClicked);

        //creamos la lista de datos para el CarouselView de la toolbar
        ToolbarItems = new ObservableCollection<ToolbarItem>
        {
            new ToolbarItem{ Text = "Texto", IconImageSource = "text_icon.png", Command = addTextCommand},
            new ToolbarItem{ Text = "Imagen", IconImageSource = "image_icon.png", Command = addImageCommand },
            new ToolbarItem{ Text = "Checklist",IconImageSource = "checklist_icon.png", Command = addChecklistCommand },
            new ToolbarItem{ Text = "Multiple", IconImageSource = "multiple_icon.png", Command = addMultipleChoiceCommand },
            new ToolbarItem{ Text = "Única", IconImageSource = "unique_icon.png", Command = addSingleSelectionCommand },
        };

        this.BindingContext = this;

        LoadFormToEdit(formId); //cargamos los datos.
    }

    private async void OnElementTapped(object sender, TappedEventArgs e)
    {
        // Guarda el elemento seleccionado(el contenedor Grid)
        var selectedContainer = sender as Grid;
        if (selectedContainer == null) return;

        _selectedElement = selectedContainer;

        //Reinicia el tamaño y color de todos los frames
        foreach (var child in CanvasGrid.Children)
        {
            if (child is Grid container)
            {
                //Buscamos el border dentro del Grid contenedor
                var border = container.Children.FirstOrDefault(c => c is Border) as Border;
                if (border != null)
                {
                    border.Scale = 1.0; // Tamaño normal
                    border.Stroke = Colors.LightGray; // Color normal
                    border.StrokeThickness = 1; // Grosor normal
                }
            }
        }
        var selectedBorder = selectedContainer.Children.FirstOrDefault(c => c is Border) as Border;
        if (selectedBorder != null)
        {
            selectedBorder.Scale = 1.0; // Tamaño normal
            selectedBorder.Stroke = Colors.DodgerBlue; // Cambia el color del borde para resaltar
            selectedBorder.StrokeThickness = 3; // Aumenta el grosor del borde
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

    private void CreateAndAddElement(View mainContent, string existingTitle = "")
    {
        //crea un nuevo Entry para el titulo
        var titleInput = new Entry
        {
            Text = existingTitle,
            Placeholder = "Ingrese el titulo",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black,
        };

        //crea el boton eliminar
        var deleteButton = new ImageButton
        {
            Source = "delete_icon2.png",
            WidthRequest = 24,
            HeightRequest = 24,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(0, -8, -8, 0),
            ZIndex = 1,
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

        //envuelve el grid en un frame
        var borderElement = CreateBorderElement(innerGrid);
        borderElement.ZIndex = 0; //el frame va detras

        //define la accion del boton eliminar
        deleteButton.Clicked += OnDeleteButtonClicked;

        //crea un contenedor absoluto para el frame y el boton eliminar
        var containerGrid = new Grid();

        containerGrid.Children.Add(borderElement);

        containerGrid.Children.Add(deleteButton); // agrega el boton al contenedor

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
            TextColor = Colors.CornflowerBlue,
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

    private void OnAddSingleSelectionClicked()
    {
        //crea un contenedor vertical
        var singleSelectionStack = new VerticalStackLayout { Spacing = 5 };

        //Asegura que los RadioButtons de esta pregunta no se mezclen con otros
        string groupName = Guid.NewGuid().ToString();

        //Crea el boton para añadir mas opciones
        var addItemButton = new Button
        {
            Text = "Agregar Opción",
            WidthRequest = 120,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.CornflowerBlue,
            HorizontalOptions = LayoutOptions.Start,
        };

        //Añade la fila con RadioButton mas Entry
        addItemButton.Clicked += (s, args) =>
        {
            var newItemLayout = new HorizontalStackLayout { Spacing = 5 };

            //Creamos el RadioButton y le asignamos el grupo único
            var radioButton = new RadioButton { GroupName = groupName };

            var entry = new Entry
            {
                Placeholder = "Opción",
                FontSize = 16,
                TextColor = Colors.Black,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 200
            };

            newItemLayout.Children.Add(radioButton);
            newItemLayout.Children.Add(entry);

            //Inserta antes del boton de agregar
            singleSelectionStack.Children.Insert(singleSelectionStack.Children.Count - 1, newItemLayout);
        };

        // Crea la primera opción por defecto
        var firstItemLayout = new HorizontalStackLayout { Spacing = 5 };
        var firstRadioButton = new RadioButton { GroupName = groupName, IsChecked = true }; //Marcamos la primera por defecto
        var firstEntry = new Entry
        {
            Placeholder = "Opción 1",
            FontSize = 16,
            TextColor = Colors.Black,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200
        };

        firstItemLayout.Children.Add(firstRadioButton);
        firstItemLayout.Children.Add(firstEntry);

        //Añade todo al contenedor
        singleSelectionStack.Children.Add(firstItemLayout);
        singleSelectionStack.Children.Add(addItemButton);

        //Lo manda al lienzo
        CreateAndAddElement(singleSelectionStack);
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
            newOptionLayout.Children.Add(new Entry { Placeholder = "Nueva opción", VerticalOptions = LayoutOptions.Center, });

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

    private void ResetForm()
    {
        if (FormTitleEntry != null)
            FormTitleEntry.Text = string.Empty;
        if (FormDescriptionEditor != null)
            FormDescriptionEditor.Text = string.Empty;

        CanvasGrid.Children.Clear();

        CanvasGrid.RowDefinitions.Clear();

        _nextRow = 0;
        _selectedElement = null;
    }

    private async void UpdateFormAsync(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FormTitleEntry.Text))
        {
            await DisplayAlert("Error", "El título del formulario no puede estar vacío.", "OK");
            return;
        }

        string userIdString = await SecureStorage.Default.GetAsync("user_id");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int idUser))
        {
            await DisplayAlert("Error", "No se pudo obtener la información del usuario.", "OK");
            return;
        }

        var formDto = new Models.FormDto
        {
            IdUser = idUser,
            Title = FormTitleEntry.Text,
            Description = FormDescriptionEditor.Text,
            Elements = new List<Models.FormElementDto>()
        };

        // Recorremos los hijos del CanvasGrid para extraer los elementos del formulario
        foreach (var child in CanvasGrid.Children)
        {
            // agregamos el elemento a la lista del formulario
            if (child is Grid containerGrid)
            {
                //Buscamos el borde dentro del Grid contenedor
                var border = containerGrid.Children.FirstOrDefault(c => c is Border) as Border;

                if (border != null && border.Content is Grid innerGrid)
                {

                    //obtener el titulo (siempre esta en la fila 0)
                    var titleEntry = innerGrid.Children
                    .OfType<Entry>()
                    .FirstOrDefault(c => Grid.GetRow(c) == 0);

                    string elementType = "Desconocido";
                    string combinedOptions = "";

                    //obtener el Contenido (siempre esta en la fila 1)
                    //Usamos (BindableObject) para evitar el error de compilacion
                    var content = innerGrid.Children.FirstOrDefault(c => Grid.GetRow((BindableObject)c) == 1);

                    //Determinar el tipo
                    if (content is Label) elementType = "Texto";
                    else if (content is Button) elementType = "Imagen";
                    else if (content is Entry) elementType = "Campo de Entrada";
                    else if (content is VerticalStackLayout stack)
                    {
                        bool isUnique = stack.Children.OfType<HorizontalStackLayout>()
                            .Any(row => row.Children.Any(c => c is RadioButton));

                        if (isUnique) elementType = "Seleccion Unica";
                        else elementType = "Checklist/Multiple";

                        //extraer las opciones escritas por el usuario
                        var optionsList = new List<string>();
                        foreach (var child2 in stack.Children)
                        {
                            if (child2 is HorizontalStackLayout row)
                            {
                                var optionsentry = row.Children.OfType<Entry>().FirstOrDefault();
                                if (optionsentry != null && !string.IsNullOrWhiteSpace(optionsentry.Text))
                                {
                                    optionsList.Add(optionsentry.Text.Trim());
                                }
                            }
                        }

                        //Unimos la lista con comas.
                        combinedOptions = string.Join(",", optionsList);
                    }

                    //Agregar a la lista de elementos del formulario
                    formDto.Elements.Add(new Models.FormElementDto
                    {
                        Title = titleEntry?.Text ?? "Sin titulo",
                        Type = elementType,
                        Options = combinedOptions
                    });
                }
            }
        }

        var apiService = new ApiService();
        bool isSuccess = await apiService.UpdateFormAsync(_formId, formDto);

        if (isSuccess)
        {
            await DisplayAlert("Éxito", "El formulario se ha guardado correctamente.", "OK");
            ResetForm();
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await DisplayAlert("Error", "Hubo un problema al guardar el formulario. Por favor, inténtelo de nuevo.", "OK");
        }
    }

    private async void LoadFormToEdit(int id)
    {
        var api = new ApiService();
        var form = await api.GetFormDetailsAsync(id);
        if (form == null) return;

        //cargamos los datos del formulario en la interfaz
        FormTitleEntry.Text = form.Title;
        FormDescriptionEditor.Text = form.Description;

        //Dibujamos los elementos.
        foreach (var element in  form.Elements)
        {
            if (element.Type == "Texto" || element.Type == "Campo de Entrada")
            {
                var newLabel = new Label { Text = "Nuevo Texto", Padding = 5, FontSize = 16, TextColor = Colors.Black };
                CreateAndAddElement(newLabel, element.Title);
            }
            else if (element.Type == "Imagen")
            {
                var newBtn = new Button { Text = "Inserte una imagen", WidthRequest = 200, HeightRequest = 50 };
                CreateAndAddElement(newBtn, element.Title);
            }
            else if (element.Type.Contains("Unica") || element.Type.Contains("Multiple") || element.Type.Contains("CheckList"))
            {
                var stack = new VerticalStackLayout { Spacing = 5 };
                string groupName = Guid.NewGuid().ToString();

                //Si tiene opciones guardadas las separamos.
                string[] options = string.IsNullOrWhiteSpace(element.Options) ? new string[0] : element.Options.Split(',');

                foreach (var op in  options)
                {
                    var row = new HorizontalStackLayout { Spacing = 5 };
                    if (element.Type.Contains("Unica")) row.Children.Add(new RadioButton { GroupName = groupName });
                    else row.Children.Add(new CheckBox());

                    row.Children.Add(new Entry { Text = op.Trim(), FontSize =16, TextColor =Colors.Black, 
                        VerticalOptions = LayoutOptions.Center, WidthRequest = 200});
                    stack.Children.Add(row);
                }

                //Agregamos el boton para permitir añadir mas opciones durante la edicion.
                var addItemButton = new Button { Text = "Agregar Opcion", WidthRequest = 120, HeightRequest=40, 
                    BackgroundColor = Colors.Transparent, TextColor = Colors.CornflowerBlue, HorizontalOptions = LayoutOptions.Start};
                addItemButton.Clicked += (s, args) =>
                {
                    var newRow = new HorizontalStackLayout {Spacing = 5 };
                    if (element.Type.Contains("Unica")) newRow.Children.Add(new RadioButton { GroupName = groupName });
                    else newRow.Children.Add(new CheckBox());
                    newRow.Children.Add(new Entry {Placeholder = "Nueva Opción", FontSize = 16, TextColor = Colors.Black, 
                        VerticalOptions = LayoutOptions.Center, WidthRequest = 200 });
                    stack.Children.Insert(stack.Children.Count - 1, newRow);
                };
                stack.Children.Add(addItemButton);

                CreateAndAddElement(stack, element.Title);
            }
        }
    }
}