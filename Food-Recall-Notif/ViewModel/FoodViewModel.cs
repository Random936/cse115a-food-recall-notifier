using System.Collections.ObjectModel;
using System.Diagnostics;
using Food_Recall_Notif.Model;
using Food_Recall_Notif.Services;

namespace Food_Recall_Notif.ViewModel;

public partial class FoodViewModel : BaseViewModel
{
    readonly FoodService foodService;
    public ObservableCollection<Food_Item> Food_Items { get; } = new();

    public FoodViewModel(FoodService foodService)
    {
        Title = "Food Finder";
        this.foodService = foodService;
    }
    async Task GetFoodAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var food_items = await foodService.GetFood();

            if (Food_Items.Count != 0) Food_Items.Clear();
            foreach (var food in food_items)
            {
                Food_Items.Add(food);
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
