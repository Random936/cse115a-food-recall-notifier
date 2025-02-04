using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

public class ServerStatus
{
    public string name { get; set; }
    public string version { get; set; }
    public string db_state { get; set; }
    public int last_modified { get; set; }
}

namespace MauiApp1
{

    public class ClientAPI
    {
        public ServerStatus Items { get; private set; }
        public async Task<ServerStatus> ClientAPIMain() {
            HttpClient _client = new HttpClient();
            Uri uri = new Uri("https://notifier-api.randomctf.com/");

        JsonSerializerOptions _serializerOptions;
        
      
            
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
    }
}
