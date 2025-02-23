using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

public partial class FoodViewModel : BaseViewModel
{
    private readonly FoodService foodService;

    // Observable Collections
    public ObservableCollection<Food_Item> DefaultResults { get; set; } = [];
    public ObservableCollection<Food_Item> BarcodeResults { get; } = [];
    public ObservableCollection<Food_Item> SearchResults { get; } = [];


    // Visibility Properties
    [ObservableProperty]
    private bool isDefaultVisible;

    [ObservableProperty]
    private bool isSearchListVisible;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isBusy;
    [ObservableProperty]
    string searchText;

    // Constructor
    public FoodViewModel(FoodService foodService)
    {
        Title = "Food Finder";
        this.foodService = foodService;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        IsDefaultVisible = true;
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

        if (IsBusy) return;
        Debug.Write($"search query: {searchQuery}");
        if (string.IsNullOrWhiteSpace(searchQuery))
        {

            IsSearchListVisible = false;
            IsDefaultVisible = true;
            SearchResults.Clear();
            return;
        }

        try
        {
            IsBusy = true;
            IsSearchListVisible = true;
            IsDefaultVisible = false;

            var searchResult = await foodService.SearchUPC(searchQuery) ?? [];
            Debug.Write($"search results: {searchResult}");
            SearchResults.Clear();


            foreach (var food in searchResult)
            {
                SearchResults.Add(food);
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
