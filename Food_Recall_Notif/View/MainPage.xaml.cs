using Food_Recall_Notif.Services;
using Plugin.LocalNotification;

namespace Food_Recall_Notif.View
{
    public partial class MainPage : ContentPage
    {
        private readonly NotificationService notificationService;
        private readonly int count = 0;

        // Constructor to initialize the MainPage and bind the ViewModel and services
        public MainPage(FoodViewModel _viewModel, NotificationService notificationService)
        {
            InitializeComponent();
            BindingContext = _viewModel;
            this.notificationService = notificationService;
            _ = InitializeDataAsync();  // Asynchronously initialize data
        }
        // Handle Sort button click event
        private async void OnSortButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var sortOption = button?.Text;
            var _viewModel = (FoodViewModel)BindingContext;
            if (sortOption != null)
            {
                await _viewModel.SortItems(sortOption);
            }
        }
        // Navigate to BarcodeReaderPage when camera button is clicked
        private async void Button_CameraButtonClicked(object sender, EventArgs e)
        {
            var _viewModel = (FoodViewModel)BindingContext;
            _viewModel.HasSearched = false;
            _viewModel.BarcodeResults.Clear();
            var barcodePage = App.Services.GetRequiredService<BarcodeReaderPage>();
            await Navigation.PushAsync(barcodePage);
        }

        // Handle Home button click event to reset the view to the default state
        private void HomeButtonClicked(object sender, EventArgs e)
        {
            if (IsBusy) return;  // Prevent further actions if the page is busy

            try
            {
                IsBusy = true;

                // Access the ViewModel and update the CurrentItems with DefaultResults
                var _viewModel = (FoodViewModel)BindingContext;
                _viewModel.CurrentItems.Clear();
                foreach (var food in _viewModel.DefaultResults)
                {
                    _viewModel.CurrentItems.Add(food);
                }
                _viewModel.DefaultView = true;  // Reset to default view
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);  // Log any exceptions for debugging
            }
            finally
            {
                IsBusy = false;  // Ensure the page is no longer busy
            }
        }
        // Handle Home button click event to search for only product type food
        private void FoodButtonClicked(object sender, EventArgs e)
        {
            if (IsBusy) return;  // Prevent further actions if the page is busy

            try
            {
                IsBusy = true;

                // Access the ViewModel and update the CurrentItems with DefaultResults
                var _viewModel = (FoodViewModel)BindingContext;
                var filteredItems = _viewModel.CurrentItems.Where(item => item.product_type == "Food").ToList();
                _viewModel.CurrentItems.Clear();
                foreach (var item in filteredItems)
                {
                    _viewModel.CurrentItems.Add(item);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);  // Log any exceptions for debugging
            }
            finally
            {
                IsBusy = false;  // Ensure the page is no longer busy
            }
        }

        // Initialize data asynchronously by fetching and comparing the server status
        private async Task InitializeDataAsync()
        {
            try
            {
                Debug.WriteLine("Loading old file...");
                ServerStatus? oldStatus = await notificationService.LoadDataFromFileAsync();  // Load old data from file
                Debug.WriteLine("Fetching new server data...");
                ServerStatus newStatus = await notificationService.FetchServerStatus();  // Fetch new server data
                if (newStatus != null)
                {
                    Debug.WriteLine($"New data fetched. Checking for changes...");
                    bool isDifferent = oldStatus == null || IsDatabaseDifferent(oldStatus, newStatus);  // Check if the database has changed

                    if (isDifferent)
                    {
                        Debug.WriteLine("Database has changed! Sending notification...");
                        // Create a set of product descriptions from the old data
                        HashSet<string> oldDescriptions = oldStatus?.newest.Select(item => item.product_description).ToHashSet() ?? [];

                        // Find new recalls that aren't in the old descriptions list
                        var newRecalls = newStatus.newest.Where(item => !oldDescriptions.Contains(item.product_description)).ToList();
                        if (newRecalls.Count > 0)
                        {
                            Debug.WriteLine("New recalls detected! Displaying new items:");
                            foreach (var item in newRecalls)
                            {
                                // Send notifications for new recalls
                                SendNotification(item.product_description, item.reason_for_recall, "Recalled");
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No changes detected.");
                    }

                    // Save the new status to file for future comparisons
                    await notificationService.SaveDataToFileAsync(newStatus);
                }
                else
                {
                    Debug.WriteLine("New data is null. No update performed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in InitializeDataAsync: {ex.Message}");  // Log any exceptions
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        // Compare if the old database and new database are different
        private static bool IsDatabaseDifferent(ServerStatus oldData, ServerStatus newData)
        {
            if (oldData == null || newData == null || oldData.newest == null || newData.newest == null)
                return true;  // Return true if either old or new data is null

            // Serialize both the old and new data to JSON and compare them
            string oldJson = JsonSerializer.Serialize(oldData.newest);
            string newJson = JsonSerializer.Serialize(newData.newest);

            return !oldJson.Equals(newJson);  // Return true if data is different
        }

        // Send a push notification with the provided title, message, and subtitle
        private void SendNotification(string title, string message, string subtitle)
        {
            Random random = new();
            int notificationId = random.Next(1000, 9999);  // Generate a random notification ID
            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Subtitle = subtitle,
                Description = message,
                BadgeNumber = count  // Keep track of notifications count
            };

            // Show the notification using the local notification center
            LocalNotificationCenter.Current.Show(request);
            Debug.WriteLine("Notification sent!");  // Log that the notification was sent
        }
    }
}