using MongoDB.Driver;
using Food_Recall_Notif.Model;

namespace Food_Recall_Notif.Services;
public class FoodService
{
    private readonly IMongoCollection<Food_Item> _foodCollection;

    // Constructor to initialize the MongoDB client and collection
    public FoodService(string connectionString, string databaseName, string collectionName)
    {
        var client = new MongoClient(connectionString); // Connect to MongoDB
        var database = client.GetDatabase(databaseName); // Access the database
        _foodCollection = database.GetCollection<Food_Item>(collectionName); // Access the collection
    }
    public async Task<List<Food_Item>> GetFood()
    {
        return await _foodCollection.Find(_ => true).ToListAsync(); // Get all documents
    }
}