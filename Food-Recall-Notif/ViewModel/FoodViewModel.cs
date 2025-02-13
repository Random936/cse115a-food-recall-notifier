using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Food_Recall_Notif.Model;
using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

public partial class FoodViewModel : BaseViewModel
{
    readonly FoodService foodService;
    public ObservableCollection<Food_Item> Food_Items { get; } = [];

    public FoodViewModel(FoodService foodService)
    {
        Title = "Food Finder";
        this.foodService = foodService;
    }
    [RelayCommand]
    async Task GetFoodAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var food_items = await foodService.GetAll();

            if (Food_Items.Count != 0) Food_Items.Clear();
            foreach (var food in food_items)
            {
                Food_Items.Add(food);
                Debug.WriteLine($"Item: Brand={food.Brand}, Date={food.Date}");
                Debug.WriteLine($"Food items count: {food_items?.Count ?? 0}");

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!", "Unable to get food", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
