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



//A class that contains name, version, newest list of recalled items, and last_modified.
//So if a ServerStatus variable is made, you can ddo variable.name to get the "FRN-Server" on the website.
public class ServerStatus
{
    public string name { get; set; }
    public string version { get; set; }
    public List<RecallItem> newest { get; set; } // Change from string to List<RecallItem>
    public string last_modified { get; set; }
}

//this is another class that lets you access its status, product_description (basically the name), reason, and the report date. 
public class RecallItem
{
    //if u look at the website, https://notifier-api.randomctf.com/, in the newest section you can see the format. I've excluded some of the other information for its not really needed for notifications.
    public string status { get; set; }
    public string product_description { get; set; } //basically the name
    public string reason_for_recall { get; set; } //reason

    public string report_date { get; set; }

}


namespace MauiApp1
{
    //Class because I guess I needed it for each file?
    public class ClientAPI
    {
        
        public ServerStatus Items { get; private set; }
        private readonly HttpClient _client; //setting up httpclient using WebRequestMethods I'm pretty sure. 
        private readonly JsonSerializerOptions _serializerOptions; //declaring the serializer option.
        private readonly string _filePath; //declaring the filepath which the file is saved.
        public ClientAPI() //main function
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            // Define file path in app storage
            string appDataDirectory = FileSystem.AppDataDirectory; //default app directory, based off of NET MAUI.
            _filePath = Path.Combine(appDataDirectory, "server_status.json"); 
        }
        //This function does basically a get on the specified website, and 
        public async Task<ServerStatus> FetchServerStatus() {
            HttpClient _client = new HttpClient();
            Uri uri = new Uri("https://notifier-api.randomctf.com/"); //website its pulling from "like a get" For checking if the database has been changed. And use these to check for new items https://notifier-api.randomctf.com/search/upc/all?count=1 
            
            
            JsonSerializerOptions _serializerOptions;
            // Define file path in app storage incase the main isn't called.
            string appDataDirectory = FileSystem.AppDataDirectory; //default AppDataDirectory.
            string _filePath = Path.Combine(appDataDirectory, "server_status.json"); 
            //below is a print statement incase we wanna see the exact path to find the file in the phone.
            //Debug.WriteLine("File Path: " + _filePath);
  
            //making the options again incase main wasn't called.
        _serializerOptions = new JsonSerializerOptions
        {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };


        try
        {
                Debug.WriteLine("before first await");
                //
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
                //just a really long debug line to seeif something broke.
            Debug.WriteLine(@"\tERROR {0} THIS IS THE ERROR MESSAGE UR LOOKIGN FOR ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss", ex.Message);
        }
        //return the seralization and deserialization of the data. To make it into the classes stated above. 
        return Items;
    }
        //saves the serverstatus class into the filepath in appDataDirectory variable. 
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

        //loads the data from the file from JSON and converts it into serverstatus type.
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
            //return null if it doesn't exist.
            return null;
        }
    }
}
