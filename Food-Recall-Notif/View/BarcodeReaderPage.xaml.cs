namespace Food_Recall_Notif.View
{
    public partial class BarcodeReaderPage : ContentPage
    {
        private readonly FoodViewModel viewModel;
        public BarcodeReaderPage()
        {
            InitializeComponent(); // Ensure this is called
            BindingContext = viewModel;
        }

        private void BarcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            Dispatcher.Dispatch(() =>
       {
           // Get the first barcode detected
           string upcCode = e.Results[0].Value;
           if (upcCode != null)
           {
               viewModel?.PerformSearchCommand.Execute(upcCode);
               // Optionally stop scanning after detecting a barcode
               barcodeReader.IsDetecting = false;
           }
       });
        }
    }
}