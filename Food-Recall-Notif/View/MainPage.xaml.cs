using Food_Recall_Notif.ViewModel;

namespace Food_Recall_Notif.View;

public partial class MainPage : ContentPage
{
	public MainPage(FoodViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;


	}
	private async void Button_CameraButtonClicked(object sender, System.EventArgs e)
	{
		// Navigate to the BarcodeReaderPage
		await Navigation.PushAsync(new BarcodeReaderPage());
	}

	private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
	{
		// Search logic can be implemented here
		// Example: Filter RecallItems based on search input
	}

	private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
	{
		var searchBar = (SearchBar)sender;
		string searchQuery = searchBar.Text;
		//TODO: search in db.json
	}

}

