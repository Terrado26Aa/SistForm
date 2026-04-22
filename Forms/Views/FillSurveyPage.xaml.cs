using Forms.Services;
using Forms.Models;
namespace Forms.Views;

public partial class FillSurveyPage : ContentPage
{
    private int _formId;
    private string _editingLocalId = null; // Para identificar si estamos editando una respuesta local o creando una nueva
    private List<View> _inputControls = new List<View>(); // Lista para almacenar los controles de entrada dinamicos
    public FillSurveyPage(int formId, string editingLocalId = null)
    {
        InitializeComponent();
        _formId = formId;
        _editingLocalId = editingLocalId;
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
            //Dibujar cada elemento del formulario en la interfaz de usuario.

            //Si estamos editando una respuesta local, cargamos las respuestas anteriores para mostrarlas en la interfaz.
            SubmitResponseDto draftToEdit = null;
            if (!string.IsNullOrEmpty(_editingLocalId))
            {
                var pendingList = await LocalDatabaseHelper.GetPendingResponsesAsync();
                draftToEdit = pendingList.FirstOrDefault(x => x.LocalId == _editingLocalId);
            }

            foreach (var element in form.Elements)
            {

                //Si la pregunta esta corrupta o vacia, la salta.
                if(element == null) continue;

                //Buscamos si esta pregunta ya tenia respuesta para mostrarla en la interfaz.
                string savedAnswer = "";
                if (draftToEdit != null && draftToEdit.Responses != null)
                {
                    var prevResponse = draftToEdit.Responses.FirstOrDefault(r => r.FormElementId == element.Id);
                    if (prevResponse != null)
                    {
                        savedAnswer = prevResponse.Answer ?? "";
                    }
                }

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

                //Normalizamos el texto (minusculas y sin espacios ocultos)
                string secureType = element.Type.Trim().ToLower() ?? "";

                if (secureType == "texto" || secureType == "campo de entrada")
                {
                    inputControl = new Entry
                    {
                        Placeholder = "Escriba su respuesta",
                        FontSize = 14,
                        Text = savedAnswer // Cargar la respuesta guardada si existe
                    };
                }
                //Corregir aqui una vez resuelta lo de la base de datos.
                else if (secureType.Contains("lista") || secureType.Contains("unica") || secureType.Contains("multiple") || 
                    secureType.Contains("checklist"))
                {
                    var optionStack = new VerticalStackLayout { Spacing = 5 };

                    //Separar las opciones por comas.
                    string[] options = string.IsNullOrEmpty(element.Options)
                        ? new string[0] 
                        : element.Options.Split(',');

                    string radioGroup = Guid.NewGuid().ToString(); //para que los radios no se mezclen.

                    //Separamos las respuestas guardadas en una lista limpia y sin espacios extra.
                    string[] savedOptions = string.IsNullOrEmpty(savedAnswer)
                        ? new string[0]
                        : savedAnswer.Split(",").Select(s => s.Trim()).ToArray();

                    foreach (var option in options)
                    {
                        string cleanOption = option.Trim(); //Limpiamos la opcion actual
                        var row = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };
                        //Creamos la etiqueta de la opcion
                        var lblOption = new Label { Text = option, VerticalOptions = LayoutOptions.Center };

                        //Verificamos si esta opcion estaba seleccionada en la respuesta guardada para marcarla en la interfaz.
                        bool isChecked = !string.IsNullOrEmpty(savedAnswer) && savedAnswer.Contains(option.Trim());

                        //Si es seleccion unica RadioButton
                        if (secureType.Contains("unica"))
                        {
                            var rb = new RadioButton { GroupName = radioGroup, Value = cleanOption, IsChecked = isChecked };
                            row.Children.Add(rb);
                        }
                        //Si es multiple o checklist (CheckBox)
                        else
                        {
                            var cb = new CheckBox {IsChecked = isChecked};
                            row.Children.Add(cb);

                            //activamos el limite solamente si incluye la palabra "multiple"
                            if (secureType.Contains("multiple"))
                            {
                                //Leemos el limite de la base de datos (o ponemo 99 si no hay limite)
                                int maxLimit = (element.MaxSelections != null && element.MaxSelections >0) ? element.MaxSelections.Value : 99;

                                cb.CheckedChanged += (sender, e) =>
                                {
                                    //Solo revisamos cuando el usuario intenta marcar la casilla
                                    if (e.Value)
                                    {
                                        int brand = 0;

                                        //contamos cuantas casillas hay marcadas en esta pregunta especifica.
                                        foreach (var childRow in optionStack.Children.OfType<HorizontalStackLayout>())
                                        {
                                            var interCb = childRow.Children.OfType<CheckBox>().FirstOrDefault();
                                            if(interCb != null && interCb.IsChecked) brand++;
                                        }

                                        //Si se paso el limite establecido.
                                        if(brand > maxLimit)
                                        {
                                            //Desmarcamos esta casilla inmediatamente.
                                            ((CheckBox)sender).IsChecked = false;

                                            //Mostramos la alerta para el usuario.
                                            Application.Current.MainPage.DisplayAlert("Limite alcanzado", 
                                                $"La pregunta solo permite un maximo de {maxLimit} opciones.", "OK");
                                        }
                                    }
                                };
                            }
                        }

                        row.Children.Add(lblOption);
                        optionStack.Children.Add(row);
                    }

                    inputControl = optionStack;
                }
                else if(secureType == "imagen")
                {
                    var imageStack = new VerticalStackLayout { Spacing = 10 };

                    //La imagen donde mostraremos la vista previa (oculta al principio)
                    var previewImage = new Image { HeightRequest = 200, IsVisible = false, Aspect = Aspect.AspectFit };

                    //El boton para seleccionar la imagen
                    var pickButton = new Button { Text = "Seleccionar Imagen", BackgroundColor = Colors.CornflowerBlue};

                    //Un Entry oculto para almacenar la imagen en base64 (no es la mejor forma, pero es una solucion rapida para no complicar el ejemplo con clases adicionales)
                    var hiddenBase64Label = new Label { IsVisible = false, Text = ""};

                    if(!string.IsNullOrEmpty(savedAnswer))
                    {
                        try
                        {
                            byte[] imgBytes = Convert.FromBase64String(savedAnswer);
                            previewImage.Source = ImageSource.FromStream(() => new MemoryStream(imgBytes));
                            previewImage.IsVisible = true;
                            pickButton.Text = "Cambiar Imagen";
                            hiddenBase64Label.Text = savedAnswer; // Cargar la imagen en base64 en el Entry oculto
                        }
                        catch
                        {
                            // Si ocurre un error al cargar la imagen, simplemente dejamos el estado inicial (sin imagen)
                        }
                    }

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

                                // Convertimos la imagen a base64 y la almacenamos en el Entry oculto para enviarla luego al backend
                                hiddenBase64Label.Text = Convert.ToBase64String(memoryStream.ToArray());
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
                    imageStack.Children.Add(hiddenBase64Label);
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
                    var hiddenLabel = stack.Children.OfType<Label>().FirstOrDefault(l => !l.IsVisible);
                    answer = hiddenLabel?.Text ?? ""; // Obtener el valor del Entry oculto que contiene la imagen en base64
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
            UserId = finalUserId,
            Responses = awnsers,
            FormTitle = this.Title,
            SaveAt = DateTime.Now
        };

        try
        {
            if(!string.IsNullOrEmpty(_editingLocalId))
            {
                //Si estamos editando una respuesta local, eliminamos la versión anterior para reemplazarla por la nueva.
                await LocalDatabaseHelper.DeletePendingResponseAsync(_editingLocalId);
            }

            //Guardamos la version nueva y actualizada de la respuesta localmente, ya sea que estemos editando una
            //respuesta local existente o creando una nueva.
            await LocalDatabaseHelper.SaveResponseLocallyAsync(submitDto);

            await DisplayAlert("Guardado local", "Tus respuestas han sido guardadas localmente y se enviarán cuando " +
                "tengas conexión a internet.", "OK");
            await Navigation.PopAsync(); //Volver a la página anterior después de guardar las respuestas localmente.
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar localmente: {ex.Message}", "OK");
        }
    }
}