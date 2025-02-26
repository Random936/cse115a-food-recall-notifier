using System.Threading.Tasks;
using Food_Recall_Notif.Services;
using Food_Recall_Notif.ViewModel;

namespace Food_Recall_Notif.View;

public partial class MainPage : ContentPage
{
	private readonly FoodService foodService;

	public MainPage(FoodViewModel _viewModel, FoodService foodService)
	{
		InitializeComponent();
		BindingContext = _viewModel;
		this.foodService = foodService;

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

