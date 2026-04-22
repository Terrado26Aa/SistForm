using Forms.Services;
using Forms.Models;
namespace Forms.Views;

public partial class FillSurveyPage : ContentPage
{
    private int _formId;
    private List<View> _inputControls = new List<View>(); // Lista para almacenar los controles de entrada dinamicos
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

            Title = "Cargando encuesta...";

            //Conexion online con la encuesta.
            try
            {
                if(Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var api = new ApiService();
                    var apiTask = api.GetFormDetailsAsync(id);

                    if(await Task.WhenAny(apiTask, Task.Delay(5000))== apiTask)
                    {
                        form = await apiTask;
                        //Guardamos la encuesta en el celular para que este disponible offline.
                        if (form != null) await LocalDatabaseHelper.SaveFormLocallyAsync(form);
                    }
                    else
                    {
                        //Si la consulta tarda mas de 5 segundos, asumimos que hay un problema de conexion y
                        //tratamos de cargar la encuesta offline.
                    }

                    if (form != null) await LocalDatabaseHelper.SaveFormLocallyAsync(form);
                }
            }
            catch { }

            //Recupera el formulario de forma Offline.
            if(form == null)
            {
                //corregir el error
                form = await LocalDatabaseHelper.GetDownloadedFormByIdAsync(id);
            }

            if(form == null)
            {
                await DisplayAlert("Aviso", "No tienes conexión y este formulario no esta guardado en tu dispositivo", "OK");
                await Navigation.PopAsync();
                return;
            }

            Title = form.Title ?? "Encuesta sin titulo";

            if (FormContainer == null) return;
            FormContainer.Children.Clear();

            //Evitamos crash si Elements viene null o vacio
            if(form.Elements == null || !form.Elements.Any())
            {
                await DisplayAlert("Info", "Este formulario no tiene preguntas para responder", "OK");
                return;
            }

            //AutoReparación: si olvidamos crear la lista arriba, la creamos aqui.
            if(_inputControls == null)
            {
                _inputControls = new List<View>();
            }
            _inputControls.Clear(); //Limpiamos por si la pagina se carga dos veces.
            //Dibujar cada elemento del formulario en la interfaz de usuario
            foreach (var element in form.Elements)
            {

                //Si la pregunta esta corrupta o vacia, la salta.
                if(element == null) continue;

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
            await DisplayAlert("Error Detallado", $"Mensaje: {ex.Message}\n\nLínea: {ex.StackTrace}", "OK");
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