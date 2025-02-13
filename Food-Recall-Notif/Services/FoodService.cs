using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Food_Recall_Notif.Model;

namespace Food_Recall_Notif.Services
{
    public class FoodService
    {
        HttpClient httpClient;
        public FoodService()
        {
            this.httpClient = new HttpClient();
        }
        List<Food_Item> foodlist;
        public async Task<List<Food_Item>?> GetAll()
        {
            if (foodlist?.Count > 0)
                return foodlist;

            var response = await httpClient.GetAsync("https://notifier-api.randomctf.com/search/upc/all");
            if (response.IsSuccessStatusCode)
            {
                foodlist = await response.Content.ReadFromJsonAsync<List<Food_Item>>() ?? new List<Food_Item>();

            }

            return foodlist ?? [];
        }
    }
}
