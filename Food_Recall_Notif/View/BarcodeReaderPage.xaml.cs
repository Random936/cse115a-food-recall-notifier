using ZXing.Net.Maui;

namespace Food_Recall_Notif.View
{
    public partial class BarcodeReaderPage : ContentPage
    {
        private readonly FoodViewModel viewModel;

        // Inject FoodViewModel via constructor
        public BarcodeReaderPage(FoodViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;

            // Configure scanner settings
            ConfigureScanner();
        }

        private void ConfigureScanner()
        {
            barcodeReader.Options = new BarcodeReaderOptions()
            {
                //AutoRotate = true,
                //TryHarder = true
                //UseCode39ExtendedMode = true
                //Formats = ZXing.Net.Maui.BarcodeFormat.UpcE // Adjust formats as needed
            };

            // Apply camera orientation adjustment based on device orientation
            AdjustCameraOrientation();
        }

        private void AdjustCameraOrientation()
        {
            // Get the current device orientation
            var displayInfo = DeviceDisplay.MainDisplayInfo;

            // Calculate the rotation angle based on the device orientation
            double rotationAngle = 0;

            if (displayInfo.Orientation == DisplayOrientation.Portrait)
            {
                rotationAngle = 90; // Reverse portrait orientation
            }
            else if (displayInfo.Orientation == DisplayOrientation.Landscape)
            {
                rotationAngle = 0; // Reverse landscape orientation
            }

            // Apply the rotation to the camera preview (barcodeReader)
            barcodeReader.Rotation = rotationAngle;
        }

        private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                string upcCode = e.Results[0].Value;
                if (upcCode != null)
                {
                    DisplayAlert("Barcode Detected", $"UPC Code: {upcCode}", "OK");
                    viewModel?.PerformBarcodeSearchCommand.Execute(upcCode);

                    barcodeReader.IsDetecting = false;  // Stop detecting after reading
                }
            });
        }
    }
}
