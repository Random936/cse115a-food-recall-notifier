using Food_Recall_Notif.ViewModel;

namespace Food_Recall_Notif.View;

public partial class MainPage : ContentPage
{
	public MainPage(FoodViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;


	}
	private async void Button_CameraButtonClicked(object sender, System.EventArgs e)
	{
		// Navigate to the BarcodeReaderPage
		await Navigation.PushAsync(new BarcodeReaderPage());
	}

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

