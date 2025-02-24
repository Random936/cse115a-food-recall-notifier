using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

public partial class FoodViewModel : BaseViewModel
{
    private readonly FoodService foodService;

    // Observable Collections
    public ObservableCollection<Food_Item> DefaultResults { get; set; } = [];

    public ObservableCollection<Food_Item> CurrentItems { get; set; } = [];

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isBusy;
    [ObservableProperty]
    string searchText;

    // Constructor
    public FoodViewModel(FoodService foodService)
    {
        this.foodService = foodService;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await GetFoodAsync();
    }

    [RelayCommand]
    private async Task GoToDetails(Food_Item foodItem)
    {
        if (foodItem is null) return;
        await Shell.Current.GoToAsync($"{nameof(View.FoodDetailsPage)}?upc={foodItem.upc}");
    }

    [RelayCommand]
    private async Task GetFoodAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var foodItems = await foodService.GetAll() ?? [];

            DefaultResults.Clear();
            if (foodItems == null) return;

            foreach (var food in foodItems)
            {
                DefaultResults.Add(food);
            }

            CurrentItems.Clear();
            foreach (var food in DefaultResults)
            {
                CurrentItems.Add(food);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task PerformSearch(string searchQuery)
    {
        Debug.WriteLine("search");
        if (IsBusy) return;
        try
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                CurrentItems.Clear();
                foreach (var food in DefaultResults)
                {
                    CurrentItems.Add(food);
                }
            }
            else
            {
                var searchResult = await foodService.SearchUPC(searchQuery) ?? [];

                CurrentItems.Clear();

                foreach (var food in searchResult)
                {
                    CurrentItems.Add(food);
                }
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


}
