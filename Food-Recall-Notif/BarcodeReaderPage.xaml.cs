namespace Food_Recall_Notif
{
    public partial class BarcodeReaderPage : ContentPage
    {
        public BarcodeReaderPage()
        {
            InitializeComponent(); // Ensure this is called
        }

        private void BarcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            Dispatcher.Dispatch(() =>
       {
           // Get the first barcode detected
           var barcode = e.Results.FirstOrDefault();
           if (barcode != null)
           {
               // Display the barcode value
               DisplayAlert("Barcode Detected", $"Barcode Value: {barcode.Value}", "OK");

               // Optionally stop scanning after detecting a barcode
               barcodeReader.IsDetecting = false;
           }
       });
        }
    }
}