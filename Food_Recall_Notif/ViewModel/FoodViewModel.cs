using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

public partial class FoodViewModel : BaseViewModel
{
    // FoodService to interact with the data layer for food recall information
    private readonly FoodService foodService;

    // Observable collections to hold food items and search results
    public ObservableCollection<Food_Item> DefaultResults { get; set; } = [];
    public ObservableCollection<Food_Item> CurrentItems { get; set; } = [];
    public required ObservableCollection<Food_Item> BarcodeResults;

    // Properties to manage UI states like refreshing and busy indicators
    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isBusy;

    // Properties to manage view state (default view) and pagination offsets
    [ObservableProperty]
    bool defaultView;
    [ObservableProperty]
    int defaultOffset;
    [ObservableProperty]
    int searchOffset;
    [ObservableProperty]
    bool hasSearched;


    // Backing field for SearchText, used in search functionality
    private string _searchText = " ";

    public string SearchText
    {
        get => _searchText;
        set
        {
            // Update search text and trigger property change notification
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
    }

    // Property for selected sort option used to sort the food items
    private string _selectedSortOption = string.Empty;
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (_selectedSortOption != value)
            {
                _selectedSortOption = value;
                // Call the SortItems async function
                OnSelectedSortOptionChangedAsync(_selectedSortOption);
                OnPropertyChanged();
            }
        }
    }

    // Dependencies for connectivity and geolocation services
    readonly IConnectivity connectivity;
    readonly IGeolocation geolocation;

    // Constructor to initialize services and begin data retrieval
    public FoodViewModel(FoodService foodService, IConnectivity connectivity, IGeolocation geolocation)
    {
        this.foodService = foodService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;

        // Begin initialization process asynchronously
        _ = InitializeAsync();
    }

    // Asynchronous method to initialize data by fetching food items
    private async Task InitializeAsync()
    {
        await GetFoodAsync();
    }
    private async void OnSelectedSortOptionChangedAsync(string sortOption)
    {
        await SortItems(sortOption);
    }
    // Command to navigate to the details page for a selected food item
    [RelayCommand]
    private async Task GoToDetails(Food_Item foodItem)
    {
        // Return if no food item is selected
        if (foodItem is null) return;

        // Check for internet connectivity before navigating
        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Could not connect to server!", "Please check internet and try again.", "OK");
            return;
        }

        // Navigate to the food details page with the selected food item recall number
        await Shell.Current.GoToAsync($"{nameof(View.FoodDetailsPage)}?recall_number={foodItem.recall_number}");
    }

    // Command to retrieve food items from the service asynchronously
    [RelayCommand]
    private async Task GetFoodAsync()
    {
        // Prevent simultaneous execution of the method
        if (IsBusy) return;

        try
        {
            // Check for network connectivity before attempting to retrieve data
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!", "Please check internet and try again.", "OK");
                return;
            }

            // Set busy state and reset offset for default food items
            IsBusy = true;
            DefaultOffset = 0;

            // Retrieve the list of food items from the service
            var foodItems = await foodService.GetAll(DefaultOffset) ?? [];

            // Clear previous food items and add the new ones
            DefaultResults.Clear();
            if (foodItems == null) return;

            foreach (var food in foodItems)
            {
                DefaultResults.Add(food);
            }

            // Clear current items and populate with the fetched food items
            CurrentItems.Clear();
            foreach (var food in DefaultResults)
            {
                CurrentItems.Add(food);
            }

            // Set default view flag to true
            DefaultView = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            // Reset busy and refreshing states after the operation
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    // Command to perform a search for food items based on the search query
    [RelayCommand]
    private async Task PerformSearch(string searchQuery)
    {
        // Prevent simultaneous execution of the method
        if (IsBusy) return;

        try
        {
            // Check for network connectivity before attempting to retrieve search results
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!", "Please check internet and try again.", "OK");
                return;
            }

            // Reset search offset
            SearchOffset = 0;

            // Perform search using the query and update the current items collection
            var searchResult = await foodService.SearchProduct(searchQuery, SearchOffset) ?? [];
            CurrentItems.Clear();

            foreach (var food in searchResult)
            {
                CurrentItems.Add(food);
            }
            DefaultView = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            // Reset the busy state after the search operation
            IsBusy = false;
        }
    }
    [RelayCommand]
    private async Task PerformBarcodeSearch(string barcode)
    {
        // Prevent simultaneous execution of the method
        if (IsBusy)
        {
            return;
        }
        try
        {
            // Check for network connectivity before attempting to retrieve search results
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!", "Please check internet and try again.", "OK");
                return;
            }
            // Reset search offset
            SearchOffset = 0;
            // If the search query is empty, display all food items
            if (string.IsNullOrWhiteSpace(barcode))
            {
                await Shell.Current.DisplayAlert("Invalid Barcode", "Please enter a valid barcode and try again.", "OK");
            }
            else
            {
                // Perform search using the query and update the current items collection
                
                var searchResult = await foodService.SearchUpc(barcode) ?? [];
                HasSearched = true;
                BarcodeResults.Clear();
                if (searchResult.Count == 0) return;
                foreach (var food in searchResult)
                {
                    Debug.WriteLine(food);
                    BarcodeResults.Add(food);
                }

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            // Reset the busy state after the search operation
            IsBusy = false;
        }
    }
    // Command to load more items either from the default or search view
    [RelayCommand]
    private async Task LoadMoreItems()
    {
        // Check if the default view is active
        if (DefaultView)
        {
            // Check for network connectivity before loading more food items
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Could not connect to server!", "Please check internet and try again.", "OK");
                return;
            }

            Debug.WriteLine("Default View");

            // Increase the offset and fetch additional food items
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

            // Increase the search offset and fetch more search results
            SearchOffset += 30;
            var searchResult = await foodService.SearchProduct(SearchText, SearchOffset) ?? [];

            foreach (var food in searchResult)
            {
                CurrentItems.Add(food);
            }
        }
    }

    // Method to sort the current items based on the selected sort option
    public async Task SortItems(string selectedSort)
    {
        // Return if the sort option is empty
        if (string.IsNullOrEmpty(selectedSort)) return;

        // Create a copy of the current items list for sorting
        var sortedList = CurrentItems.ToList();

        // Sort the items based on the selected sort option
        switch (selectedSort)
        {
            case "Brand A-Z":
                sortedList = [.. sortedList.OrderBy(f => f.recalling_firm)];
                break;
            case "Brand Z-A":
                sortedList = [.. sortedList.OrderByDescending(f => f.recalling_firm)];
                break;
            case "Newest-Oldest":
                sortedList = [.. sortedList.OrderByDescending(f => f.Date)];
                break;
            case "Nearest-Furthest":
                sortedList = await SortByDistance();
                break;
        }
        CurrentItems.Clear();
        foreach (var item in sortedList)
        {
            CurrentItems.Add(item);
        }
    }
    private static readonly Dictionary<string, string> StateAbbreviations = new()
    {
        {"Alabama", "AL"}, {"Alaska", "AK"}, {"Arizona", "AZ"}, {"Arkansas", "AR"},
        {"California", "CA"}, {"Colorado", "CO"}, {"Connecticut", "CT"}, {"Delaware", "DE"},
        {"Florida", "FL"}, {"Georgia", "GA"}, {"Hawaii", "HI"}, {"Idaho", "ID"},
        {"Illinois", "IL"}, {"Indiana", "IN"}, {"Iowa", "IA"}, {"Kansas", "KS"},
        {"Kentucky", "KY"}, {"Louisiana", "LA"}, {"Maine", "ME"}, {"Maryland", "MD"},
        {"Massachusetts", "MA"}, {"Michigan", "MI"}, {"Minnesota", "MN"}, {"Mississippi", "MS"},
        {"Missouri", "MO"}, {"Montana", "MT"}, {"Nebraska", "NE"}, {"Nevada", "NV"},
        {"New Hampshire", "NH"}, {"New Jersey", "NJ"}, {"New Mexico", "NM"}, {"New York", "NY"},
        {"North Carolina", "NC"}, {"North Dakota", "ND"}, {"Ohio", "OH"}, {"Oklahoma", "OK"},
        {"Oregon", "OR"}, {"Pennsylvania", "PA"}, {"Rhode Island", "RI"}, {"South Carolina", "SC"},
        {"South Dakota", "SD"}, {"Tennessee", "TN"}, {"Texas", "TX"}, {"Utah", "UT"},
        {"Vermont", "VT"}, {"Virginia", "VA"}, {"Washington", "WA"}, {"West Virginia", "WV"},
        {"Wisconsin", "WI"}, {"Wyoming", "WY"}
    };
    private async Task<List<Food_Item>> SortByDistance()
    {
        if (CurrentItems.Count == 0) return [.. CurrentItems];

        try
        {
            // Get user's current location
            var location = await geolocation.GetLastKnownLocationAsync()
                ?? await geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });

            if (location == null)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve location", "OK");
                return [.. CurrentItems];
            }

            // Reverse geocode to get the user's state
            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var userState = placemarks?.FirstOrDefault()?.AdminArea; // Typically holds the state name or abbreviation

            if (string.IsNullOrEmpty(userState))
            {
                await Shell.Current.DisplayAlert("Error", "Unable to determine your state", "OK");
                return [.. CurrentItems];
            }
            string userStateAbbr = StateAbbreviations.TryGetValue(userState, out var abbr) ? abbr : userState;
            // Filter recalls to only include those that match the user's state
            var filteredRecalls = CurrentItems
                .Where(r => r.ParsedStates.Any(state => state.Equals(userStateAbbr, StringComparison.OrdinalIgnoreCase)))
                .ToList();


            return filteredRecalls;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to sort by distance: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            return [.. CurrentItems];
        }
    }



}
