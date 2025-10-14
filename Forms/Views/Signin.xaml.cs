namespace Forms.Views;

public partial class Signin : ContentPage
{
	public Signin()
	{
		InitializeComponent();
	}

	private async void OnLoginClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"//{nameof(Login)}");
	}
}