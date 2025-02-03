using System.Collections.Generic;

namespace Food_Recall_Notif;

public partial class MainPage : ContentPage
{
	public List<RecallItem> RecallItems { get; set; } // List to hold the items

	public MainPage()
	{
		InitializeComponent();

		// Populate the RecallItems list
		RecallItems = new List<RecallItem>
		{
			new RecallItem { Title = "Food item", Details = "Details about the food item", PageType = typeof(FoodDetailPage) },
			new RecallItem { Title = "Drug item", Details = "Details about the drug item", PageType = typeof(DrugDetailPage) }
		};

		// Set the BindingContext for data binding
		BindingContext = this;
	}

	private async void OnItemTapped(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is RecallItem selectedItem)
		{
			// Navigate to the page specified in the item's PageType
			if (selectedItem.PageType != null)
			{
				Page? detailPage = (Page?)Activator.CreateInstance(selectedItem.PageType);
				if (detailPage != null)
				{
					// Handle the created page
					await Navigation.PushAsync(detailPage);
				}
				else
				{
					// Handle the case where the page could not be created
					await DisplayAlert("Error", "Page creation failed", "OK");
				}
			}
		}
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
		Dispatcher.Dispatch(() =>
		{
			DisplayAlert("Search", "Search button pressed", "OK");
		});
	}
}

// Class to represent recall items
public class RecallItem
{
	public required string Title { get; set; }
	public required string Details { get; set; }
	public required Type PageType { get; set; } // Type of the page to navigate to
}
