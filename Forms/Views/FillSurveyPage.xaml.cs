using Forms.Services;
using Forms.Models;
namespace Forms.Views;

public partial class FillSurveyPage : ContentPage
{
	private int _formId;
	private List<View> _inputControls = new List<View>(); // Lista para almacenar los controles de entrada dinámicos
    public FillSurveyPage(int formId)
	{
		InitializeComponent();
        _formId = formId;
        LoadForm(formId);
    }

    private async void LoadForm(int id)
    {
        var api = new ApiService();
        var form = await api.GetFormDetailsAsync(id);//obtener el formulario por id

        Title = form.Title;
        FormContainer.Children.Clear(); // Un VerticalStackLayout para contener los elementos del formulario

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
            if (element.Type == "Texto" ||  element.Type == "Campo de Entrada")
            {
                inputControl = new Entry
                {
                    Placeholder = "Escribe tu respuesta aquí",
                    FontSize = 14
                };
            }
            else if (element.Type == "Lista" || element.Type == "Checklist")
            {
                inputControl = new Picker
                {
                    Title = "Selecciona una opción",
                    FontSize = 14
                };
                // Aquí podrías agregar opciones al Picker si las tienes en el elemento
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

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var awnsers = new List<ResponseDetailDto>();
        foreach (var View in _inputControls)
        {
            var elementData = View.BindingContext as FormElementDto;
            string answer = "";

            if (View is Entry entry) answer = entry.Text;
            else if (View is Picker picker) answer = picker.SelectedItem?.ToString() ?? "";
            //Aquí añadir lógica para Checkbox/Radio si los implementas

            awnsers.Add(new ResponseDetailDto
            {
                FormElementId = elementData.Id,
                Answer = answer
            });
        }

        var submitDto = new SubmitResponseDto
        {
            FormId = _formId,
            UserId = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            Responses = awnsers
        };

        var api = new ApiService();
        await api.SubmitResponseAsync(submitDto);
        await DisplayAlert("Éxito", "Tus respuestas han sido enviadas.", "OK");
        await Navigation.PopAsync(); // Volver a la página anterior
    }
}