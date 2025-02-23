using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

public partial class FoodViewModel : BaseViewModel
{
    private readonly FoodService foodService;

    // Observable Collections
    public ObservableCollection<Food_Item> DefaultResults { get; set; } = [];
    public ObservableCollection<Food_Item> BarcodeResults { get; } = [];

    private readonly ObservableCollection<Food_Item> _defaultResults = [];
    private readonly ObservableCollection<Food_Item> _searchResults = [];

    private ObservableCollection<Food_Item> _sortedResults;
    public ObservableCollection<Food_Item> SearchResults
    {
        get => _sortedResults;
        set
        {
            _sortedResults = value;
            OnPropertyChanged();
        }
    }

    // Visibility Properties
    [ObservableProperty]
    private bool isDefaultVisible;

    [ObservableProperty]
    private bool isSearchListVisible;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isBusy;

    private string _selectedSortOption;
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (_selectedSortOption != value)
            {
                _selectedSortOption = value;
                try
                {
                    ApplySorting(IsSearchListVisible);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sorting Error: {ex.Message}");
                }
                OnPropertyChanged();
            }
        }
    }

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

        await Shell.Current.GoToAsync($"{nameof(View.FoodDetailsPage)}",
            true,
            new Dictionary<string, object> { { "Food_Item", foodItem } });
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

            _searchResults.Clear();
            if (searchResult == null)
            {
                IsSearchListVisible = false;
                IsDefaultVisible = true;
                return;
            }

            foreach (var food in searchResult)
            {
                _searchResults.Add(food);
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

    private void ApplySorting(bool isSearchView)
    {
        var targetList = isSearchView ? _searchResults : _defaultResults;

        if (targetList == null || !targetList.Any())
        {
            if (isSearchView)
                SearchResults = [];
            else
                DefaultResults = [];
            return;
        }

        if (SelectedSortOption == "Brand (A-Z)")
        {
            SetSortedResults(isSearchView, targetList.OrderBy(item => item.brand));
        }
        else if (SelectedSortOption == "Brand (Z-A)")
        {
            SetSortedResults(isSearchView, targetList.OrderByDescending(item => item.brand));
        }
        else if (SelectedSortOption == "Newest First")
        {
            SetSortedResults(isSearchView, targetList.OrderByDescending(item => item.date));
        }
        else if (SelectedSortOption == "Oldest First")
        {
            SetSortedResults(isSearchView, targetList.OrderBy(item => item.date));
        }
    }

    private void SetSortedResults(bool isSearchView, IEnumerable<Food_Item> sortedItems)
    {
        if (isSearchView)
            SearchResults = [.. sortedItems];
        else
            DefaultResults = [.. sortedItems];
    }
}
