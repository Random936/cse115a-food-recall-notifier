namespace Food_Recall_Notif;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(View.FoodDetailsPage), typeof(View.FoodDetailsPage));
	}
}
