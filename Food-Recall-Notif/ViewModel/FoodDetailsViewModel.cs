using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

[QueryProperty(nameof(Food_Item), "Food_Item")]
public partial class FoodDetailsViewModel(FoodService foodService) : BaseViewModel
{
    readonly FoodService foodService = foodService;
    [ObservableProperty]
    public required Food_Item food_item;
    [ObservableProperty]
    public required UPC_Item upc_item;

    partial void OnFood_itemChanged(Food_Item value)
    {
        if (value?.upc is not null)
        {
            // Start loading details asynchronously
            _ = LoadUpcItemDetailsAsync(value.upc);
        }
    }
    [RelayCommand]
    async Task LoadUpcItemDetailsAsync(string upc)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            UPC_Item? upc_item = await foodService.GetUPCItem(upc);
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