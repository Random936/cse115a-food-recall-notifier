using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
namespace Food_Recall_Notif.Platforms.Android;
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    // This is called when the app starts
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // For Android 13+ (API 33+), explicitly request permission to send notifications
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Check if running on Android 13+
        {
            RequestPermissions([Manifest.Permission.PostNotifications], 0);
        }

        // Create a notification channel, required for Android 8.0+ (API 26+)
        CreateNotificationChannel();
    }

    // Sets up a notification channel for Android 8.0+ (required to send notifications)
    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) // Check if running on Android 8.0+
        {
            // Define a notification channel ID, name, description, and importance level
            var channel = new NotificationChannel(
                "default_channel", // Channel ID
                "General Notifications", // Channel Name (visible to users)
                NotificationImportance.Default) // Importance level (e.g., low, default, high)
            {
                Description = "Default notification channel" // Description of the channel
            };

            // Get the system notification manager and register the channel
            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }
}