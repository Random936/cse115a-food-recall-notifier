using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using System.Diagnostics;
using System.Text.Json;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        
        private static readonly string FileName = "db_state.json"; // Local file name
        private ClientAPI client;
        JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

    public MainPage()
        {

           InitializeComponent();
            client = new ClientAPI();

            // Run async method properly
            _ = InitializeDataAsync();
            //SendNotification("Name and Date", "Reason", "Recall Notification");

        }

        private async Task InitializeDataAsync()
        {
            try
            {
                //ServerStatus dummy;
        //        string dummyJson = @"
        //{
        //    ""name"": ""FRN-Server"",
        //    ""version"": ""v0.1"",
        //    ""newest"": [
        //        {
        //            ""country"": ""United States"",
        //            ""city"": ""New York"",
        //            ""address_1"": ""123 Main Street"",
        //            ""reason_for_recall"": ""Spinach recall due to potential E. coli contamination."",
        //            ""address_2"": """",
        //            ""product_quantity"": ""10 cases"",
        //            ""code_info"": ""Lot # SP-202502"",
        //            ""center_classification_date"": ""20250215"",
        //            ""distribution_pattern"": ""Distributed across multiple states in the U.S."",
        //            ""state"": ""NY"",
        //            ""product_description"": ""Fresh Organic Baby Spinach - 5 oz. bag"",
        //            ""report_date"": ""20250220"",
        //            ""classification"": ""Class I"",
        //            ""openfda"": {},
        //            ""recalling_firm"": ""Healthy Greens Inc."",
        //            ""recall_number"": ""F-1234-2025"",
        //            ""initial_firm_notification"": ""Email, Letter, Press Release"",
        //            ""product_type"": ""Food"",
        //            ""event_id"": ""100101"",
        //            ""termination_date"": """",
        //            ""more_code_info"": """",
        //            ""recall_initiation_date"": ""20250210"",
        //            ""postal_code"": ""10001"",
        //            ""voluntary_mandated"": ""Voluntary: Firm initiated"",
        //            ""status"": ""Ongoing""
        //        }
        //    ],
        //    ""last_modified"": ""20250225""
        //}";
                //dummy = JsonSerializer.Deserialize<ServerStatus>(dummyJson, _serializerOptions);
                Debug.WriteLine("Loading old file...");
                ServerStatus oldStatus = await client.LoadDataFromFileAsync();

                Debug.WriteLine("Fetching new server data...");
                ServerStatus newStatus = await client.FetchServerStatus();
                Debug.WriteLine($"Old: ");

                foreach (var item in oldStatus.newest)
                {
                    Debug.WriteLine($"Product: {item.product_description}");
                }

                Debug.WriteLine($"New: ");
                // Extract product descriptions

                foreach (var item in newStatus.newest)
                {
                    Debug.WriteLine($"Product: {item.product_description}");
                }




                if (newStatus != null)
                {
                    Debug.WriteLine($"New data fetched. Checking for changes...");

                    bool isDifferent = IsDatabaseDifferent(oldStatus, newStatus);

                    if (isDifferent)
                    {
                        Debug.WriteLine("Database has changed! Sending notification...");
                        //SendNotification("Product Recall Alert!", "New recalls have been added.", "");
                        var oldDescriptions = new HashSet<string>(oldStatus.newest.Select(item => item.product_description));
                        var newRecalls = newStatus.newest.Where(item => !oldDescriptions.Contains(item.product_description)).ToList();

                        if (newRecalls.Count > 0)
                        {
                            Debug.WriteLine("New recalls detected! Displaying new items:");
                            foreach (var item in newRecalls)
                            {
                                //Debug.WriteLine($"[NEW RECALL] Product: {item.product_description}");
                                SendNotification(item.product_description, item.reason_for_recall, "Recalled");
                            }
                        }

                    }
                    else
                    {
                        Debug.WriteLine("No changes detected.");
                    }

                    // Save the latest data to file
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
            if (oldData == null || newData == null)
                return true; // If old data doesn't exist, treat it as different

            if (oldData.newest == null || newData.newest == null)
                return true; // If db_state is missing, treat as different

            // Compare JSON representations to detect changes
            string oldJson = JsonSerializer.Serialize(oldData.newest);
            string newJson = JsonSerializer.Serialize(newData.newest);

            return !oldJson.Equals(newJson);
        }

        /// <summary>
        /// Sends a local notification when new recalls are detected.
        /// </summary>
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

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            var request = new NotificationRequest
            {
                NotificationId = count,
                Title = count == 1 ? "Pineapples 1/23/25" : "Yes",
                Subtitle = count == 1 ? "" : "YES",
                Description = count == 1 ?
                    "Pineapples have been recalled due to salmonella contamination." :
                    "YES!!!",
                BadgeNumber = count
            };

            LocalNotificationCenter.Current.Show(request);
            CounterBtn.Text = $"Clicked {count} times";
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
