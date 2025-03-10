using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

[QueryProperty("Recall_number", "recall_number")]
public partial class FoodDetailsViewModel(FoodService foodService) : BaseViewModel
{
    private readonly FoodService? foodService = foodService;
    [ObservableProperty]
    public UPC_Item? upcItem;
    [ObservableProperty]
    string? recall_number;

    [RelayCommand]
    public async Task LoadUpcItemDetailsAsync(string recall_number)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (foodService == null)
            {
                return;
            }
            UpcItem = await foodService.GetUPCItem(recall_number);
            if (UpcItem == null)
            {
                Debug.Write($"No recall number found for {recall_number}\n");
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
