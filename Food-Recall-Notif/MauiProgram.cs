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
				fonts.AddFont("FluentSystemIcons-Filled.ttf", "FluentIcons");
			})
			.UseBarcodeReader();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
