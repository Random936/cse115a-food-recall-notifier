namespace Food_Recall_Notif;
public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCameraButtonClicked(object sender, System.EventArgs e)
	{
		// Navigate to the BarcodeReaderPage
		await Navigation.PushAsync(new BarcodeReaderPage());
	}
}

