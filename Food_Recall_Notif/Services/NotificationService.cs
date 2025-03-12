namespace Food_Recall_Notif.Services
{
    public class NotificationService
    {
        public required ServerStatus Items { get; set; }
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string _filePath;
        public NotificationService()
        {

            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string appDataDirectory = FileSystem.AppDataDirectory;
            _filePath = Path.Combine(appDataDirectory, "notifications.json");
        }
        public async Task<ServerStatus> FetchServerStatus()
        {
            HttpClient _client = new HttpClient();
            Uri uri = new Uri("https://notifier-api.randomctf.com/"); //website its pulling from "like a get" For checking if the database has been changed. And use these to check for new items https://notifier-api.randomctf.com/search/upc/all?count=1 


            // Define file path in app storage incase the main isn't called.
            string appDataDirectory = FileSystem.AppDataDirectory; //default AppDataDirectory.
            _ = Path.Combine(appDataDirectory, "server_status.json");
            //below is a print statement incase we wanna see the exact path to find the file in the phone.
            //Debug.WriteLine("File Path: " + _filePath);


            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Items = JsonSerializer.Deserialize<ServerStatus>(content, _serializerOptions) ?? throw new InvalidOperationException("Failed to deserialize ServerStatus.");
                }
                Debug.WriteLine("Finished get");
            }
            catch (Exception ex)
            {
                //just a really long debug line to see if something broke.
                Debug.WriteLine(@"\tERROR {0} THIS IS THE ERROR MESSAGE UR LOOKIGN FOR", ex.Message);
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
                await File.WriteAllTextAsync(_filePath, jsonString);

                Debug.WriteLine($"Data saved to: {_filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        //loads the data from the file from JSON and converts it into serverstatus type.
        public async Task<ServerStatus?> LoadDataFromFileAsync()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string jsonString = await File.ReadAllTextAsync(_filePath);
                    return JsonSerializer.Deserialize<ServerStatus>(jsonString, _serializerOptions) ?? throw new InvalidOperationException("Failed to deserialize ServerStatus.");
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