using Forms.Services;
using Forms.Models;
namespace Forms.Views;

public partial class FillSurveyPage : ContentPage
{
    private int _formId;
    private List<View> _inputControls = new List<View>(); // Lista para almacenar los controles de entrada din�micos
    public FillSurveyPage(int formId)
    {
        InitializeComponent();
        _formId = formId;
        LoadForm(formId);
    }

    private async void LoadForm(int id)
    {
        try
        {
            FormDto form = null;

            //Verificar si hay internet.
            var currentStack = Connectivity.Current.NetworkAccess;

            if (currentStack == NetworkAccess.Internet)
            {
                //Intentamos cargar desde la API.
                var api = new ApiService();
                form = await api.GetFormDetailsAsync(id);

                //Fallback offline (si no hay internet o la API Fallo).
                if (form == null)
                {
                    form = await LocalDatabaseHelper.GetDownloadedFormByIdAsync(id);
                }

                //Validacion final.
                if(form == null)
                {
                    await DisplayAlert("Error", "No se pudo cargar el formulario. Asegúrate de tener conexión a internet o " +
                        "de haber descargado el formulario previamente.", "OK");
                    await Shell.Current.GoToAsync("//HomePage"); 
                    return; // Salir del método para evitar errores posteriores.
                }
            }

            //Asignamos el titulo del formulario a la pagina, si no tiene titulo le asignamos uno por defecto
            Title = form.Title ?? "Encuesta sin titulo";

            //validar que el contenedor del formulario este disponible antes de intentar agregar controles
            if (FormContainer == null)
            {
                await DisplayAlert("Error", "El contenedor del formulario no esta disponible.", "OK");
                return; // Si el contenedor no esta disponible, salimos del metodo para evitar errores posteriores
            }

            FormContainer.Children.Clear(); // Limpiar cualquier contenido previo en el contenedor del formulario

            //validar que el formulario tenga elementos antes de intentar iterar sobre ellos
            if (form.Elements == null || form.Elements.Count == 0)
            {
                await DisplayAlert("Info", "Este formulario no tiene preguntas para responder.", "OK");
                return; // Si el formulario no tiene elementos, salimos del metodo
            }

            //dibujar cada elemento del formulario en la interfaz de usuario
            foreach (var element in form.Elements)
            {
                var label = new Label
                {
                    Text = element.Title,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };
                FormContainer.Children.Add(label);

                // Crear controles de entrada basados en el tipo de elemento
                View inputControl = null;
                if (element.Type == "Texto" || element.Type == "Campo de Entrada")
                {
                    inputControl = new Entry
                    {
                        Placeholder = "Escriba su respuesta",
                        FontSize = 14
                    };
                }
                else if (element.Type == "Lista" || element.Type == "Checklist/Multiple" || element.Type == "Seleccion Unica")
                {
                    var optionStack = new VerticalStackLayout { Spacing = 5 };

                    //Separar las opciones por comas.
                    string[] options = string.IsNullOrEmpty(element.Options)
                        ? new string[0] 
                        : element.Options.Split(',');

                    string radioGroup = Guid.NewGuid().ToString(); //para que los radios no se mezclen.

                    foreach (var option in options)
                    {
                        var row = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };

                        //Creamos la etiqueta de la opcion
                        var lblOption = new Label { Text = option, VerticalOptions = LayoutOptions.Center };

                        if (element.Type == "Seleccion Unica")
                        {
                            row.Children.Add(new RadioButton {GroupName = radioGroup, Value = option });
                        }
                        else
                        {
                            row.Children.Add(new CheckBox());
                        }

                        row.Children.Add(lblOption);
                        optionStack.Children.Add(row);
                    }

                    inputControl = optionStack;
                }
                else if(element.Type == "Imagen")
                {
                    var imageStack = new VerticalStackLayout { Spacing = 10 };

                    //La imagen donde mostraremos la vista previa (oculta al principio)
                    var previewImage = new Image { HeightRequest = 200, IsVisible = false, Aspect = Aspect.AspectFit };

                    //El boton para seleccionar la imagen
                    var pickButton = new Button { Text = "Seleccionar Imagen", BackgroundColor = Colors.CornflowerBlue};

                    //Un Entry oculto para almacenar la imagen en base64 (no es la mejor forma, pero es una solucion rapida para no complicar el ejemplo con clases adicionales)
                    var hiddenBase64Entry = new Entry { IsVisible = false, Text = ""};

                    pickButton.Clicked += async (sender, e) =>
                    {
                        try
                        {
                            var photo = await MediaPicker.Default.PickPhotoAsync();
                            if (photo != null)
                            {
                                //Mostramos la vista previa de la pantalla
                                var streamPreview = await photo.OpenReadAsync();
                                previewImage.Source = ImageSource.FromStream(() => streamPreview);
                                previewImage.IsVisible = true;
                                pickButton.Text = "Cambiar Imagen"; //Cambiar el texto del boton si ya se ha seleccionado una imagen

                                //Convertimos la imagen a base64 para almacenarla en el Entry oculto
                                using var memoryStream = new MemoryStream();
                                using var streamForBase64 = await photo.OpenReadAsync();
                                await streamForBase64.CopyToAsync(memoryStream);

                                byte[] imageBytes = memoryStream.ToArray();
                                // Convertimos la imagen a base64 y la almacenamos en el Entry oculto para enviarla luego al backend
                                hiddenBase64Entry.Text = Convert.ToBase64String(imageBytes);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    };

                    imageStack.Children.Add(previewImage);
                    imageStack.Children.Add(pickButton);
                    //Agregamos el Entry oculto al stack de la imagen para que se envíe junto con las respuestas,
                    //aunque no se muestre en la interfaz
                    imageStack.Children.Add(hiddenBase64Entry);

                    inputControl = imageStack;
                }

                if (inputControl != null)
                {
                    //Guardamos el ID del elemento en el control para saber que pregunta es
                    inputControl.BindingContext = element; // Asociar el contexto de datos
                    _inputControls.Add(inputControl); // Agregar el control a la lista
                    FormContainer.Children.Add(inputControl);
                }
            }

            var btnSend = new Button { Text = "Enviar", Margin = 20, };
            btnSend.Clicked += OnSubmitClicked;
            FormContainer.Children.Add(btnSend);
        }

        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar el formulario: {ex.Message}", "OK");
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var awnsers = new List<ResponseDetailDto>();
        foreach (var View in _inputControls)
        {
            var elementData = View.BindingContext as FormElementDto;
            string answer = "";

            if (elementData.Type == "Texto" || elementData.Type == "Campo de Entrada")
            {
                if (View is Entry entry)
                {
                    answer = entry.Text ?? "";
                }
            }
            else if (elementData.Type == "Imagen")
            {
                if (View is VerticalStackLayout stack)
                {
                    var hiddenEntry = stack.Children.OfType<Entry>().FirstOrDefault();
                    answer = hiddenEntry?.Text ?? ""; // Obtener el valor del Entry oculto que contiene la imagen en base64
                }
            }
            else
            {
                if (View is VerticalStackLayout stack) //Si es seleccion unica o multiple
                {
                    var selectedAnswers = new List<string>();

                    foreach (var row in stack.Children.OfType<HorizontalStackLayout>())
                    {
                        var checkBox = row.Children.OfType<CheckBox>().FirstOrDefault();
                        var radioButton = row.Children.OfType<RadioButton>().FirstOrDefault();
                        var label = row.Children.OfType<Label>().FirstOrDefault();

                        //Evitamos el error si no encuentra el label
                        if (label != null)
                        {
                            if (checkBox != null && checkBox.IsChecked)
                                selectedAnswers.Add(label.Text);
                            else if (radioButton != null && radioButton.IsChecked)
                                selectedAnswers.Add(label.Text);
                        }
                    }
                    answer = string.Join(", ", selectedAnswers); // Unir las respuestas seleccionadas en una sola cadena
                }
            }

            awnsers.Add(new ResponseDetailDto
            {
                FormElementId = elementData.Id,
                Answer = answer
            });
        }

        //
        string userIsString = await SecureStorage.Default.GetAsync("user_id");
        int finalUserId = 0; //0 Significa "Usuario anonimo u offline"

        if(!string.IsNullOrEmpty(userIsString))
        {
            int.TryParse(userIsString, out finalUserId);
        }

        //Empaquetamos los datos con el ID seguro.
        var submitDto = new SubmitResponseDto
        {
            FormId = _formId,
            UserId = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            Responses = awnsers
        };

        try
        {
            await LocalDatabaseHelper.SaveResponseLocallyAsync(submitDto);

            await DisplayAlert("Guardado local", "Tus respuestas han sido guardadas localmente y se enviarán cuando " +
                "tengas conexión a internet.", "OK");
            await Shell.Current.GoToAsync("//HomePage"); // Volver a la página principal después de guardar las respuestas
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar localmente: {ex.Message}", "OK");
        }
    }
}