using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using System.Diagnostics;
using System.Text.Json;

namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private static readonly string FileName = "db_state.json"; 
        private ClientAPI client;
        JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // setting a dummy US state to simulate device location -----------------------------------------------------------------------------------------
        private string deviceState = "California"; // Simulated device location
        private string deviceStateAbbr = "CA";
        // ----------------------------------------------------------------------------------------------------------------------------------------------


        public MainPage()
        {
            InitializeComponent();
            client = new ClientAPI();
            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                Debug.WriteLine("Loading old file...");
                ServerStatus oldStatus = await client.LoadDataFromFileAsync();
                Debug.WriteLine("Fetching new server data...");
                ServerStatus newStatus = await client.FetchServerStatus();

                if (newStatus != null)
                {
                    Debug.WriteLine("New data fetched. Checking for changes...");
                    bool isDifferent = IsDatabaseDifferent(oldStatus, newStatus);

                    if (isDifferent)
                    {
                        Debug.WriteLine("Database has changed! Checking for relevant recalls...");
                        var oldDescriptions = new HashSet<string>(oldStatus.newest.Select(item => item.product_description));
                        var newRecalls = newStatus.newest
                            .Where(item => !oldDescriptions.Contains(item.product_description) &&
                                           IsRecallRelevant(item.distribution_pattern))
                            .ToList();

                        if (newRecalls.Count > 0)
                        {
                            SendNotification("New Recall Issued in Your State", $"{newRecalls.Count} new recall(s) affecting {deviceState}.", "Alert"); // sending notification 
                            
                            foreach (var item in newRecalls)
                            {
                                SendNotification(item.product_description, item.reason_for_recall, "Recalled");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No relevant recalls for this state.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No changes detected.");
                    }

                    await client.SaveDataToFileAsync(newStatus);
                    Debug.WriteLine("Saved");
                }
                else
                {
                    Debug.WriteLine("New data is null. No update performed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in InitializeDataAsync: {ex.Message}");
            }
        }

        private bool IsDatabaseDifferent(ServerStatus oldData, ServerStatus newData)
        {
            if (oldData == null || newData == null || oldData.newest == null || newData.newest == null)
                return true;

            string oldJson = JsonSerializer.Serialize(oldData.newest);
            string newJson = JsonSerializer.Serialize(newData.newest);

            return !oldJson.Equals(newJson);
        }


        // Checks if US state name or abbreviation is mentioned in the distribution_pattern field of the data base ----------------------------------------
        private bool IsRecallRelevant(string distribution_pattern)
        {
            if (string.IsNullOrEmpty(distribution_pattern)) return false;
            return distribution_pattern.Contains(deviceState, StringComparison.OrdinalIgnoreCase) ||
                   distribution_pattern.Contains(deviceStateAbbr, StringComparison.OrdinalIgnoreCase);
        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------


        private void SendNotification(string title, string message, string subtitle)
        {
            Random random = new Random();
            int notificationId = random.Next(1000, 9999);
            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Subtitle = subtitle,
                Description = message,
                BadgeNumber = count
            };
            LocalNotificationCenter.Current.Show(request);
            Debug.WriteLine("Notification sent!");
        }
    }
}
