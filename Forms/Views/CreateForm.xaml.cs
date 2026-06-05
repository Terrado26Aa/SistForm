using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Forms.Services;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Forms.Views;

public partial class CreateForm : ContentPage
{
    //variable para guardar el elemento seleccionado
    private View _selectedElement;
    //variable para guardar las dimensiones iniciales del elemento
    private int _nextRow = 0;

    private ObservableCollection<ToolbarItem> _toolbarElements = new();
    public ObservableCollection<ToolbarItem> ToolbarElements 
    { 
        get => _toolbarElements; 
        set 
        { 
            _toolbarElements = value; 
            OnPropertyChanged(); 
        } 
    }

    public CreateForm()
	{
		InitializeComponent();

        //inicializa los comandos para los botones de la toolbar
        var addTextCommand = new Command(OnAddTextClicked);
        var addImageCommand = new Command(OnAddImageClicked);
        var addChecklistCommand = new Command(OnAddChecklistClicked);
        var addMultipleChoiceCommand = new Command(OnAddMultipleChoiceClicked);
        var addSingleSelectionCommand = new Command(OnAddSingleSelectionClicked);
        var addTextInputCommand = new Command(OnAddTextInputClicked);

        //creamos la lista de datos para la toolbar
        ToolbarElements = new ObservableCollection<ToolbarItem> 
        {
            new ToolbarItem{ Text = "Texto", IconImageSource = "text_icon.png", Command = addTextCommand},
            new ToolbarItem{ Text = "Imagen", IconImageSource = "image_icon.png", Command = addImageCommand },
            new ToolbarItem{ Text = "Checklist",IconImageSource = "checklist_icon.png", Command = addChecklistCommand },
            new ToolbarItem{ Text = "Multiselección", IconImageSource = "multiple_icon.png", Command = addMultipleChoiceCommand },
            new ToolbarItem{ Text = "Selección Única", IconImageSource = "unique_icon.png", Command = addSingleSelectionCommand },
            new ToolbarItem{ Text = "Campo de Entrada", IconImageSource = "input_icon.png", Command = addTextInputCommand },
        };

        // Asignamos la lista al CollectionView
        ToolbarCollection.ItemsSource = ToolbarElements;

        this.BindingContext = this;
    }

    // ─── Helper de tema: devuelve color según modo claro/oscuro ──────────────
    private static Color T(Color light, Color dark) =>
        Application.Current?.RequestedTheme == AppTheme.Dark ? dark : light;

    // Colores predefinidos del tema
    private Color TextPrimary      => T(Color.FromArgb("#111111"), Color.FromArgb("#F0F0F0"));
    private Color TextSecondary    => T(Color.FromArgb("#555555"), Color.FromArgb("#AAAAAA"));
    private Color CardBackground   => T(Colors.White, Color.FromArgb("#1E1E1E"));
    private Color BorderColor      => T(Color.FromArgb("#DDDDDD"), Color.FromArgb("#3A3A3A"));
    private Color ShadowColor      => T(Colors.Black, Colors.Black);
    private Color AccentColor      => T(Color.FromArgb("#5C7CFA"), Color.FromArgb("#7B96FF"));

    private async void OnElementTapped(object sender, TappedEventArgs e)
    {
        var selectedContainer = sender as Grid;
        if (selectedContainer == null) return;

        _selectedElement = selectedContainer;

        // Reinicia borde de todos los elementos al color del tema
        foreach (var child in CanvasGrid.Children)
        {
            if (child is Grid container)
            {
                var border = container.Children.FirstOrDefault(c => c is Border) as Border;
                if (border != null)
                {
                    border.Scale = 1.0;
                    border.Stroke = BorderColor;
                    border.StrokeThickness = 1;
                }
            }
        }
        // Resalta el elemento seleccionado
        var selectedBorder = selectedContainer.Children.FirstOrDefault(c => c is Border) as Border;
        if (selectedBorder != null)
        {
            selectedBorder.Scale = 1.0;
            selectedBorder.Stroke = Colors.DodgerBlue;
            selectedBorder.StrokeThickness = 3;
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

    private Border CreateBorderElement(View element)
    {
        return new Border
        {
            Content = element,
            Stroke = BorderColor,
            StrokeThickness = 1,
            Background = CardBackground,
            Margin = new Thickness(5),
            Padding = new Thickness(15, 10),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
            Shadow = new Shadow
            {
                Brush = ShadowColor,
                Opacity = 0.25f,
                Radius = 5,
                Offset = new Point(2, 2),
            },
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
            TextColor = TextPrimary,
            PlaceholderColor = TextSecondary,
            BackgroundColor = Colors.Transparent,
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
            Margin = new Thickness(0, -8, -8,0),
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
            TextColor = TextPrimary,
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

        //crea el botn "Agregar tem" que ira dentro del checklist
        var addItemButton = new Button
        {
            Text = "+ Agregar ítem",
            WidthRequest = 130,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = AccentColor,
            HorizontalOptions = LayoutOptions.Start,
        };

        //define la accin del botn "agregar tem" usando la funcin lambda.
        addItemButton.Clicked += (sender, e) => 
        {
            //crea un nuevo tem para la checklist (checkbox + entry)
            var newItemLayout = new HorizontalStackLayout { Spacing = 5 };
            newItemLayout.Children.Add(new CheckBox());
            newItemLayout.Children.Add(new Entry
            {
                Placeholder = "Nuevo ítem",
                FontSize = 16,
                TextColor = TextPrimary,
                PlaceholderColor = TextSecondary,
                BackgroundColor = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
            });

            //inserta el nuevo tem antes del botn "Agregar tem"
            checklistStack.Children.Insert(checklistStack.Children.Count - 1, newItemLayout);
        };

        // crea el primer tem de la checklist
        var firstItemLayout = new HorizontalStackLayout { Spacing = 5 };
        firstItemLayout.Children.Add(new CheckBox());
        firstItemLayout.Children.Add(new Entry
        {
            Placeholder = "Nuevo ítem",
            FontSize = 16,
            TextColor = TextPrimary,
            PlaceholderColor = TextSecondary,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center,
        });

        // agrega el checklistStack (con el primer tem y el botn) al canvas
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

            //Crea el boton para aadir mas opciones
            var addItemButton = new Button
            {
                Text = "+ Agregar opción",
                WidthRequest = 140,
                HeightRequest = 40,
                BackgroundColor = Colors.Transparent,
                TextColor = AccentColor,
                HorizontalOptions = LayoutOptions.Start,
            };
            
            //Aade la fila con RadioButton mas Entry
            addItemButton.Clicked += (s, args) =>
            {
                var newItemLayout = new HorizontalStackLayout { Spacing=5 };

                //Creamos el RadioButton y le asignamos el grupo nico
                var radioButton = new RadioButton { GroupName = groupName };

                var entry = new Entry
                {
                    Placeholder = "Opción",
                    FontSize = 16,
                    TextColor = TextPrimary,
                    PlaceholderColor = TextSecondary,
                    BackgroundColor = Colors.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 200
                };

                newItemLayout.Children.Add(radioButton);
                newItemLayout.Children.Add(entry);

                //Inserta antes del boton de agregar
                singleSelectionStack.Children.Insert(singleSelectionStack.Children.Count -1 , newItemLayout);
            };

            // Crea la primera opcin por defecto
            var firstItemLayout = new HorizontalStackLayout { Spacing = 5 };
            var firstRadioButton = new RadioButton { GroupName = groupName, IsChecked = true }; //Marcamos la primera por defecto
            var firstEntry = new Entry
            {
                Placeholder = "Opción 1",
                FontSize = 16,
                TextColor = TextPrimary,
                PlaceholderColor = TextSecondary,
                BackgroundColor = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 200
            };

            firstItemLayout.Children.Add(firstRadioButton);
            firstItemLayout.Children.Add(firstEntry);

            //Aade todo al contenedor
            singleSelectionStack.Children.Add(firstItemLayout);
            singleSelectionStack.Children.Add(addItemButton);

            //Lo manda al lienzo
            CreateAndAddElement(singleSelectionStack);
        }

        private void OnAddTextInputClicked()
        {
            //crea un Entry para el campo de entrada
            var textInput = new Entry
            {
                Placeholder = "Escriba su respuesta aquí",
                FontSize = 16,
                TextColor = TextPrimary,
                PlaceholderColor = TextSecondary,
                BackgroundColor = Colors.Transparent,
                Keyboard = Keyboard.Text
            };

            CreateAndAddElement(textInput);
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

            await DisplayAlert("Lmite alcanzado", $"Solo puede seleccionar hasta {limit} opciones.", "OK");
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
            TextColor = TextSecondary,
        };
        var limitEntry = new Entry
        {
            Placeholder = "Máximo de selecciones",
            Keyboard = Keyboard.Numeric,
            WidthRequest = 50,
            TextColor = TextPrimary,
            PlaceholderColor = TextSecondary,
            BackgroundColor = Colors.Transparent,
            ClassId = "MaxLimitInput"
        };

        // crea el botn para agregar opciones que ira dentro del multiple choice
        var addOptionButton = new Button
        {
            Text = "+ Agregar opción",
            WidthRequest = 140,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = AccentColor,
            HorizontalOptions = LayoutOptions.Start,
        };

        addOptionButton.Clicked += (s, args) =>
        {
            var newOptionLayout = new HorizontalStackLayout { Spacing = 5 };
            var newCheckbox = new CheckBox();

            //asigna el manejador de evento para controlar el limite de selecciones
            newCheckbox.CheckedChanged += OnCheckBoxCheckedChanged;

            newOptionLayout.Children.Add(newCheckbox);
            newOptionLayout.Children.Add(new Entry { Placeholder = "Nueva opción", TextColor = TextPrimary, PlaceholderColor = TextSecondary, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center });

            // inserta la nueva opcin antes del botn "Agregar opcin"
            multipleChoiceLayout.Children.Insert(multipleChoiceLayout.Children.Count - 1, newOptionLayout);
        };

        var firstOptionLayout = new HorizontalStackLayout { Spacing = 5 };
        var firstCheckbox = new CheckBox();
        firstCheckbox.CheckedChanged += OnCheckBoxCheckedChanged; // asigna el manejador de evento para controlar el limite de selecciones

        firstOptionLayout.Children.Add(firstCheckbox);
        firstOptionLayout.Children.Add(new Entry { Placeholder = "Nueva opción", TextColor = TextPrimary, PlaceholderColor = TextSecondary, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center });

        //aade todos los componentes al contenedor principal en orden.
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

    private async void OnSaveFormClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FormTitleEntry.Text))
        {
            await DisplayAlert("Error", "El ttulo del formulario no puede estar vaco.", "OK");
            return;
        }

        // En MacCatalyst se usa Preferences, en otros dispositivos SecureStorage
        string userIdString = DeviceInfo.Platform == DevicePlatform.MacCatalyst 
            ? Preferences.Default.Get("user_id", "") 
            : await SecureStorage.Default.GetAsync("user_id");
        
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int idUser))
        {
            await DisplayAlert("Error", "No se pudo obtener la informacin del usuario.", "OK");
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

                if(border !=  null && border.Content is Grid innerGrid)
                {

                    //obtener el titulo (siempre esta en la fila 0)
                    var titleEntry = innerGrid .Children
                    .OfType<Entry>()
                    .FirstOrDefault(c => Grid.GetRow(c) == 0);

                    string elementType = "Desconocido";
                    string combinedOptions = "";
                    int maxSelectionsLimit = 0; //variable para guardar el limite temporalmente.

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
                        else
                        {
                            // Verificamos si tiene límite de selección (es Multiple) o no (es Checklist)
                            var limitEntryControl = stack.Children.OfType<Entry>().FirstOrDefault(e => e.ClassId == "MaxLimitInput");
                            
                            if (limitEntryControl != null)
                            {
                                elementType = "Multiple"; // Tiene límite = Multiple Choice
                                if(int.TryParse(limitEntryControl.Text, out int parsedLimit))
                                {
                                    maxSelectionsLimit = parsedLimit;
                                }
                            }
                            else
                            {
                                elementType = "Checklist"; // No tiene límite = Checklist
                            }
                        }

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
                        Options = combinedOptions,
                        MaxSelections = maxSelectionsLimit //guardamos el limite en la Api
                    });
                }
            }
        }

        var apiService = new ApiService();
        var result = await apiService.SaveFormAsync(formDto);

        if (result.IsSuccess)
        {
            await DisplayAlert("Éxito", "El formulario se ha guardado correctamente.\nAhora puedes asignarlo a usuarios desde 'Gestionar Encuestas'.", "OK");
            ResetForm();
            await Shell.Current.GoToAsync("//ManageFormsPage");
        }
        else
        {
            await DisplayAlert("Error", "Hubo un problema al guardar el formulario. Por favor, inténtelo de nuevo.", "OK");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}