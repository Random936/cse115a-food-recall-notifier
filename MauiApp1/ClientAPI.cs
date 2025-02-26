using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Storage;



public class ServerStatus
{
    public string name { get; set; }
    public string version { get; set; }
    public List<RecallItem> newest { get; set; } // Change from string to List<RecallItem>
    public string last_modified { get; set; }
}

public class RecallItem
{
    public string status { get; set; }
    public string product_description { get; set; }
    public string reason_for_recall { get; set; }

    public string report_date { get; set; }

}


namespace MauiApp1
{

    public class ClientAPI
    {
            
        public ServerStatus Items { get; private set; }
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string _filePath;
        public ClientAPI()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            // Define file path in app storage
            string appDataDirectory = FileSystem.AppDataDirectory;
            _filePath = Path.Combine(appDataDirectory, "server_status.json");
        }
        public async Task<ServerStatus> FetchServerStatus() {
            HttpClient _client = new HttpClient();
            Uri uri = new Uri("https://notifier-api.randomctf.com/"); //website its pulling from "like a get" For checking if the database has been changed. And use these to check for new items https://notifier-api.randomctf.com/search/upc/all?count=1 
            
        
            JsonSerializerOptions _serializerOptions;
            // Define file path in app storage
            string appDataDirectory = FileSystem.AppDataDirectory;
            string _filePath = Path.Combine(appDataDirectory, "server_status.json");
            //Debug.WriteLine("File Path: " + _filePath);
  
            
        _serializerOptions = new JsonSerializerOptions
        {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };


        try
        {
                Debug.WriteLine("before first await");

                HttpResponseMessage response = await _client.GetAsync(uri).ConfigureAwait(false);
            
                if (response.IsSuccessStatusCode)
            {
                    Debug.WriteLine("after first await");
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Debug.WriteLine("after second await");
                    Debug.WriteLine(content);
                    Items = JsonSerializer.Deserialize<ServerStatus>(content, _serializerOptions);
            }
                Debug.WriteLine("Finished get");
            }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0} THIS IS THE ERROR MESSAGE UR LOOKIGN FOR ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss", ex.Message);
        }

        return Items;
    }

        public async Task SaveDataToFileAsync(ServerStatus data)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(data, _serializerOptions);
                await System.IO.File.WriteAllTextAsync(_filePath, jsonString);

                Debug.WriteLine($"Data saved to: {_filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving file: {ex.Message}");
            }
        }
        public async Task<ServerStatus> LoadDataFromFileAsync()
        {
            try
            {
                if (System.IO.File.Exists(_filePath))
                {
                    string jsonString = await System.IO.File.ReadAllTextAsync(_filePath);
                    return JsonSerializer.Deserialize<ServerStatus>(jsonString, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading file: {ex.Message}");
            }

            return null;
        }
    }
}
