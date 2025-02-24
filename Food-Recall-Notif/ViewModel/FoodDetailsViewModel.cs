using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

[QueryProperty("Upc", "upc")]
public partial class FoodDetailsViewModel : BaseViewModel
{
    private readonly FoodService? foodService;
    [ObservableProperty]
    public UPC_Item? upcItem;
    [ObservableProperty]
    string upc;

    public FoodDetailsViewModel(FoodService foodService)
    {
        this.foodService = foodService;
    }

    [RelayCommand]
    public async Task LoadUpcItemDetailsAsync(string upc)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (foodService == null)
            {
                return;
            }
            UpcItem = await foodService.GetUPCItem(upc);
            if (UpcItem == null)
            {
                Debug.Write($"No UPC item found for {upc}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching UPC item details: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
