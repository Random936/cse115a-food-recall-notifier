using Food_Recall_Notif.Services;
using Plugin.LocalNotification;
namespace Food_Recall_Notif.View;

public partial class MainPage : ContentPage
{
	private readonly FoodService foodService;
	private readonly NotificationService notificationService;
	int count = 0;

	public MainPage(FoodViewModel _viewModel, FoodService foodService, NotificationService notificationService)
	{

		InitializeComponent();
		BindingContext = _viewModel;
		this.foodService = foodService;
		this.notificationService = notificationService;
		_ = InitializeDataAsync();

	}
	private async void Button_CameraButtonClicked(object sender, EventArgs e)
	{
		// Navigate to the BarcodeReaderPage
		await Navigation.PushAsync(new BarcodeReaderPage());
	}
	private void HomeButtonClicked(object sender, EventArgs e)
	{
		if (IsBusy) return;

		try
		{
			IsBusy = true;

			var _viewModel = (FoodViewModel)BindingContext;
			_viewModel.CurrentItems.Clear();
			foreach (var food in _viewModel.DefaultResults)
			{
				_viewModel.CurrentItems.Add(food);
			}
			_viewModel.DefaultView = true;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}
		finally
		{
			IsBusy = false;
		}
	}
	private async Task InitializeDataAsync()
	{
		try
		{

			Debug.WriteLine("Loading old file...");
			ServerStatus? oldStatus = await notificationService.LoadDataFromFileAsync();
			Debug.WriteLine("Fetching new server data...");
			ServerStatus newStatus = await notificationService.FetchServerStatus();

			if (newStatus != null)
			{
				Debug.WriteLine($"New data fetched. Checking for changes...");
				bool isDifferent;
				if (oldStatus == null)
					isDifferent = true;
				else
					isDifferent = IsDatabaseDifferent(oldStatus, newStatus);

				if (isDifferent)
				{
					Debug.WriteLine("Database has changed! Sending notification...");
					HashSet<string> oldDescriptions;
					if (oldStatus?.newest != null)
					{
						oldDescriptions = [.. oldStatus.newest.Select(item => item.product_description)];
					}
					else
					{
						oldDescriptions = [];
					}


					var newRecalls = newStatus.newest.Where(item => !oldDescriptions.Contains(item.product_description)).ToList();
					//made a new list called newRecalls, where the items that doesn't exist in the oldStatus.newest list is added.
					if (newRecalls.Count() > 0)
					{
						Debug.WriteLine("New recalls detected! Displaying new items:");
						foreach (var item in newRecalls)
						{
							SendNotification(item.product_description, item.reason_for_recall, "Recalled");
						}
					}

				}
				else
				{
					Debug.WriteLine("No changes detected.");
				}
				await notificationService.SaveDataToFileAsync(newStatus);
			}
			else
			{
				Debug.WriteLine("New data is null. No update performed.");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error in InitializeDataAsync: {ex.Message}");
			Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
		}
	}

	private bool IsDatabaseDifferent(ServerStatus oldData, ServerStatus newData)
	{
		if (oldData == null || newData == null)
			return true;

		if (oldData.newest == null || newData.newest == null)
			return true;

		string oldJson = JsonSerializer.Serialize(oldData.newest);
		string newJson = JsonSerializer.Serialize(newData.newest);

		return !oldJson.Equals(newJson);
	}

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
	/*TODO*/
	/*async void OnUploadImageClicked(object sender, EventArgs e)
	{
		try
		{
			// Open file picker for image selection
			var photo = await MediaPicker.PickPhotoAsync();

			if (photo == null)
				return;

			// Load image into UI
			uploadedImage.Source = ImageSource.FromFile(photo.FullPath);

			// Decode UPC from image
			string upcCode = await DecodeBarcodeFromImage(photo.FullPath);

			if (!string.IsNullOrEmpty(upcCode))
				barcodeResult.Text = "UPC Code: " + upcCode;
			else
				barcodeResult.Text = "No barcode found.";
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", ex.Message, "OK");
		}
	}*/

}

