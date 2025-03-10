using System.Net.Http.Json;
namespace Food_Recall_Notif.Services
{
    public class FoodService
    {
        readonly HttpClient _client;
        public FoodService()
        {
            _client = new HttpClient();
        }
        public required List<Food_Item> foodlist;
        public async Task<List<Food_Item>?> GetAll(int offset)
        {
            var response = await _client.GetAsync($"https://notifier-api.randomctf.com/search/recall_number/all?offset={offset}&count=30&sort=desc");
            if (response.IsSuccessStatusCode)
            {
                foodlist = await response.Content.ReadFromJsonAsync<List<Food_Item>>() ?? [];

            }

            return foodlist ?? [];
        }

        public async Task<UPC_Item?> GetUPCItem(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return null;
            var response = await _client.GetAsync($"https://notifier-api.randomctf.com/query/{item}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UPC_Item>();
            }

            return null;
        }


        public required List<Food_Item> searchResult;
        public async Task<List<Food_Item>?> SearchProduct(string item, int offset)
        {
            if (string.IsNullOrWhiteSpace(item)) return null;

            var productTask = _client.GetAsync($"https://notifier-api.randomctf.com/search/product_description/{item}?offset={offset}&count=30&sort=desc");
            var firmTask = _client.GetAsync($"https://notifier-api.randomctf.com/search/recalling_firm/{item}?offset={offset}&count=30&sort=desc");

            await Task.WhenAll(productTask, firmTask);

            var productResponse = await productTask;
            var firmResponse = await firmTask;

            var searchResult = new List<Food_Item>();

            if (productResponse.IsSuccessStatusCode)
            {
                var productItems = await productResponse.Content.ReadFromJsonAsync<List<Food_Item>>() ?? [];
                searchResult.AddRange(productItems);
            }

            if (firmResponse.IsSuccessStatusCode)
            {
                var firmItems = await firmResponse.Content.ReadFromJsonAsync<List<Food_Item>>() ?? [];
                searchResult.AddRange(firmItems);
            }

            return searchResult.Any() ? searchResult : null;
        }

        public async Task<List<Food_Item>?> SearchUpc(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return null;
            Debug.WriteLine($"https://notifier-api.randomctf.com/search/upc/{item}");
            var response = await _client.GetAsync($"https://notifier-api.randomctf.com/search/upc/{item}?sort=desc");
            if (response.IsSuccessStatusCode)
            {
                searchResult = await response.Content.ReadFromJsonAsync<List<Food_Item>>() ?? [];
            }
            return searchResult; // Return null if the query fails
        }
    }
}
