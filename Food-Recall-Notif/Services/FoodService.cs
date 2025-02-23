using System.Net.Http.Json;
namespace Food_Recall_Notif.Services
{
    public class FoodService
    {
        readonly HttpClient httpClient;
        public FoodService()
        {
            httpClient = new HttpClient();
        }
        public required List<Food_Item> foodlist;
        public async Task<List<Food_Item>?> GetAll()
        {
            if (foodlist?.Count > 0)
                return foodlist;

            var response = await httpClient.GetAsync("https://notifier-api.randomctf.com/search/upc/all");
            if (response.IsSuccessStatusCode)
            {
                foodlist = await response.Content.ReadFromJsonAsync<List<Food_Item>>() ?? [];

            }

            return foodlist ?? [];
        }

        public async Task<UPC_Item?> GetUPCItem(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return null;
            var response = await httpClient.GetAsync($"https://notifier-api.randomctf.com/query/{item}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UPC_Item>();
            }

            return null;
        }


        public required List<Food_Item> searchResult;
        public async Task<List<Food_Item>?> SearchUPC(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return null;

            var response = await httpClient.GetAsync($"https://notifier-api.randomctf.com/search/brand/{item}");
            if (response.IsSuccessStatusCode)
            {
                searchResult = await response.Content.ReadFromJsonAsync<List<Food_Item>>() ?? [];

            }

            return searchResult; // Return null if the query fails
        }
    }
}
