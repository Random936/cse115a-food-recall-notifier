using System.Threading.Tasks;

namespace Food_Recall_Notif.View;

public partial class FoodDetailsPage : ContentPage
{
	public FoodDetailsPage(FoodDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

	}
	protected async override void OnAppearing()
	{
		base.OnAppearing();

		// Check if FoodItem has been set correctly
		if (BindingContext is FoodDetailsViewModel viewModel)
		{
			if (viewModel.Recall_number != null)
			{
				await viewModel.LoadUpcItemDetailsAsync(viewModel.Recall_number);
			}
			else
			{
				Debug.WriteLine("Upc is null.");
			}
		}
	}

}