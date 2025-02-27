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
    bool defaultView;
    [ObservableProperty]
    int defaultOffset;
    [ObservableProperty]
    int searchOffset;
    private string _searchText = " ";

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
    }
    private string _selectedSortOption;
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (_selectedSortOption != value)
            {
                _selectedSortOption = value;
                SortItems(_selectedSortOption);
                OnPropertyChanged();
            }
        }
    }
    IConnectivity connectivity;
    IGeolocation geolocation;

    public FoodViewModel(FoodService foodService, IConnectivity connectivity, IGeolocation geolocation)
    {
        this.foodService = foodService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;
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
        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Could not connect to server!",
                $"Please check internet and try again.", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(View.FoodDetailsPage)}?recall_number={foodItem.recall_number}");
    }

    [RelayCommand]
    private async Task GetFoodAsync()
    {
        if (IsBusy) return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!",
                    $"Please check internet and try again.", "OK");
                return;
            }
            IsBusy = true;
            DefaultOffset = 0;
            var foodItems = await foodService.GetAll(DefaultOffset) ?? [];

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
            DefaultView = true;
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
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!",
                    $"Please check internet and try again.", "OK");
                return;
            }
            SearchOffset = 0;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                CurrentItems.Clear();
                foreach (var food in DefaultResults)
                {
                    CurrentItems.Add(food);
                }
                DefaultView = true;
            }
            else
            {
                var searchResult = await foodService.SearchUPC(searchQuery, SearchOffset) ?? [];

                CurrentItems.Clear();

                foreach (var food in searchResult)
                {
                    CurrentItems.Add(food);
                }
                DefaultView = false;
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
    [RelayCommand]
    private async Task LoadMoreItems()
    {

        if (DefaultView)
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!",
                    $"Please check internet and try again.", "OK");
                return;
            }
            Debug.WriteLine("Default View");
            DefaultOffset += 30;
            var foodItems = await foodService.GetAll(DefaultOffset) ?? [];

            foreach (var food in foodItems)
            {
                CurrentItems.Add(food);
                DefaultResults.Add(food);
            }
        }
        else
        {
            Debug.WriteLine("Search View");
            SearchOffset += 30;
            var searchResult = await foodService.SearchUPC(SearchText, SearchOffset) ?? [];
            foreach (var food in searchResult)
            {
                CurrentItems.Add(food);
            }
        }
    }
    //FIX!!!!
    public void SortItems(string selectedSort)
    {
        Debug.WriteLine(selectedSort);
        if (string.IsNullOrEmpty(selectedSort)) return;

        var sortedList = CurrentItems.ToList();

        switch (selectedSort)
        {
            case "Brand A-Z":
                sortedList = sortedList.OrderBy(f => f.product_type).ToList();
                break;
            case "Brand Z-A":
                sortedList = sortedList.OrderByDescending(f => f.product_type).ToList();
                break;
            case "Newest-Oldest":
                sortedList = sortedList.OrderByDescending(f => f.Date).ToList();
                break;
            case "Oldest-Newest":
                sortedList = sortedList.OrderBy(f => f.Date).ToList();
                break;
        }

        CurrentItems.Clear();
        foreach (var item in sortedList)
        {
            CurrentItems.Add(item);
        }
    }
    [RelayCommand]
    /*async Task GetClosestRecalls()
    {
        if (IsBusy || CurrentItems.Count == 0)
            return;

        try
        {
            // Get cached location, else get real location.
            var location = await geolocation.GetLastKnownLocationAsync() ?? await geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });

            var first = CurrentItems.OrderBy(m => location.CalculateDistance(
                new Location(m.Latitude, m.Longitude), DistanceUnits.Miles));

            await Shell.Current.DisplayAlert("", first.Name + " " +
                first.Location, "OK");

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to query location: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
    }*/
}
