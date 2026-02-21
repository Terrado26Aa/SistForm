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
            var api = new ApiService();
            var form = await api.GetFormDetailsAsync(id);//obtener el formulario por id

            //validar que el formulario se haya cargado correctamente antes de intentar acceder a sus propiedades
            if (form == null)
            {
                await DisplayAlert("Error", "No se pudo cargar el formulario.", "OK");
                return; // Si no se pudo cargar el formulario, salimos del metodo para evitar errores posteriores
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

            if (View is Entry entry) answer = entry.Text ?? "";
            else if (View is VerticalStackLayout stack) //Si es seleccion unica o multiple
            {
                var selectedAnswers = new List<string>();

                foreach (var row in stack.Children.OfType<HorizontalStackLayout>())
                {
                    var checkBox = row.Children.OfType<CheckBox>().FirstOrDefault();
                    var radioButton = row.Children.OfType<RadioButton>().FirstOrDefault();
                    var label = row.Children.OfType<Label>().FirstOrDefault();

                    if (checkBox != null && checkBox.IsChecked)
                        selectedAnswers.Add(label.Text);
                    else if (radioButton != null && radioButton.IsChecked)
                        selectedAnswers.Add(label.Text);
                }
                answer = string.Join(", ", selectedAnswers); // Unir las respuestas seleccionadas en una sola cadena
            }
        }

        var submitDto = new SubmitResponseDto
        {
            FormId = _formId,
            UserId = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            Responses = awnsers
        };

        var api = new ApiService();
        await api.SubmitResponseAsync(submitDto);
        await DisplayAlert("Exito", "Tus respuestas han sido enviadas.", "OK");
        await Navigation.PopAsync(); // Volver a la p�gina anterior
    }
}