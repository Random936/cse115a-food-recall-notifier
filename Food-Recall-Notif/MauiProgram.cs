using Food_Recall_Notif.Model;
using Food_Recall_Notif.Services;
using Food_Recall_Notif.View;
using Food_Recall_Notif.ViewModel;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
namespace Food_Recall_Notif;

class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseBarcodeReader();
		// MongoDB configuration
		string connectionString = "your-mongodb-connection-string"; // Replace with actual URI
		string databaseName = "FoodDatabase";                      // Replace with your DB name
		string collectionName = "FoodCollection";                  // Replace with your collection name

		// Register FoodService with required parameters
		builder.Services.AddSingleton<FoodService>(sp =>
			new FoodService(connectionString, databaseName, collectionName));
		builder.Services.AddSingleton<FoodViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
