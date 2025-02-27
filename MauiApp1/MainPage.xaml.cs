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

           //InitializeComponent was in preset. 
           InitializeComponent();

            //create a new ClientAPI type. 
            client = new ClientAPI();

            // Run the initializedataasyn function
            _ = InitializeDataAsync();

            //below this line is just me testing the notification function, if  you wanna see how the notification format is, you can uncomment it and change the parameters, you can see the function below the code.
            //SendNotification("Name and Date", "Reason", "Recall Notification");

        }

        //summary: Basically this calls the fetch new server data function, then checks if its different. If it is, send a notification of each new item not in the file. 
        //newStatus has the new fetched data. So to access the items that are the latest 4 recalls, you would call newStatus.newest, but you may want to just print item names instead of the item itself, so you should use
        //a for loop iterating through it, and for each item you would do item.product_description.
        private async Task InitializeDataAsync()
        {
            try
            {
        //Dummy code for testing. 
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

                //Loads old file as a ServerStatus
                Debug.WriteLine("Loading old file...");
                ServerStatus oldStatus = await client.LoadDataFromFileAsync();
                //Fetches new server data from https://notifier-api.randomctf.com/
                Debug.WriteLine("Fetching new server data..."); 
                ServerStatus newStatus = await client.FetchServerStatus();
                Debug.WriteLine($"Old: ");
                
                //just a loop to print the item names in the file already on phone if there is one. Just for debugging.
                if (oldStatus != null)
                {
                    foreach (var item in oldStatus.newest)
                    {
                        Debug.WriteLine($"Product: {item.product_description}");
                    }
                }
                else
                {
                    Debug.WriteLine("No file found. Not printing each item for old.");
                }
                    Debug.WriteLine($"New: ");


                //just a loop to print the item name from the fetch. 
                foreach (var item in newStatus.newest)
                {
                    Debug.WriteLine($"Product: {item.product_description}");
                }



                //if there the website isn't down and exists.
                if (newStatus != null)
                {
                    Debug.WriteLine($"New data fetched. Checking for changes...");
                    //checks if items are different.
                    bool isDifferent = IsDatabaseDifferent(oldStatus, newStatus);

                    if (isDifferent)
                    {
                        Debug.WriteLine("Database has changed! Sending notification...");
                        
                        var oldDescriptions = new HashSet<string>(oldStatus.newest.Select(item => item.product_description));
                        
                        var newRecalls = newStatus.newest.Where(item => !oldDescriptions.Contains(item.product_description)).ToList();
                        //made a new list called newRecalls, where the items that doesn't exist in the oldStatus.newest list is added.
                        if (newRecalls.Count > 0)
                        {
                            //will only go here if there exists an item that wasn't in the old file. 
                            Debug.WriteLine("New recalls detected! Displaying new items:");
                            foreach (var item in newRecalls)
                            {
                                //Debug.WriteLine($"[NEW RECALL] Product: {item.product_description}");
                                //sends notification of the new product name and reason.
                                SendNotification(item.product_description, item.reason_for_recall, "Recalled");
                            }
                        }

                    }
                    else
                    {
                        //the new items are the same as last time checked.
                        Debug.WriteLine("No changes detected.");
                    }

                    // Save the latest data to file always after loading and checking.
                    await client.SaveDataToFileAsync(newStatus);
                    Debug.WriteLine("Saved");
                }
                else
                {
                    //newStatus is broken, website could be down or data is misformatted.
                    Debug.WriteLine("New data is null. No update performed.");
                }
            }
            catch (Exception ex)
            {
                //everything broke if printed haha...
                Debug.WriteLine($"Error in InitializeDataAsync: {ex.Message}");
            }
        }


        //function that doess a quick check if the ServerStatus datas exist, and if they do check if they are equal. 
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

       
        //function to send a notification. 
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
            //push notification
            LocalNotificationCenter.Current.Show(request);
            Debug.WriteLine("Notification sent!");
        }



        //useless code from preset is below.
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
