namespace Food_Recall_Notif.View;

public partial class FoodDetailsPage : ContentPage
{
	public FoodDetailsPage(FoodDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}