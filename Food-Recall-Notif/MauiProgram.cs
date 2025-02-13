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

		// Register services and view models
		builder.Services.AddSingleton<FoodService>();
		builder.Services.AddSingleton<FoodViewModel>();
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
