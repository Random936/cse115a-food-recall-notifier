using Food_Recall_Notif.Model;
using Food_Recall_Notif.Services;
using Food_Recall_Notif.View;
using Food_Recall_Notif.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.LocalNotification;
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
			.UseBarcodeReader()
			.UseLocalNotification();
        builder.ConfigureLifecycleEvents(events => {
		#if ANDROID                  
			events.AddAndroid(android=> android.OnCreate((activity,bundle) => MakeNavigationBarTranslucent(activity)));                  
			static void MakeNavigationBarTranslucent(Android.App.Activity activity){ 
				activity.Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#1D1D1D"));     
			}  
		#endif      
		});
		// Register services and view models
		builder.Services.AddSingleton<FoodService>();
		builder.Services.AddSingleton<NotificationService>();
		builder.Services.AddSingleton<FoodViewModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<FoodDetailsViewModel>();
		builder.Services.AddTransient<FoodDetailsPage>();
		builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
		builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
		builder.Services.AddSingleton<IMap>(Map.Default);
        builder.Services.AddTransient<BarcodeReaderPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
