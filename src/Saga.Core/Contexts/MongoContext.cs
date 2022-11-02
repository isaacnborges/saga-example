using MongoDB.Driver;

namespace Saga.Core.Contexts;
public class MongoContext : IMongoContext
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public MongoContext(string databaseName, MongoClient client)
    {
        Client = client;
        Database = GetMongoDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return Database.GetCollection<T>(collectionName);
    }

    private IMongoDatabase GetMongoDatabase(string databaseName)
    {
        try
        {
            return Client.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new MongoException("Unable to connect to MongoDB", ex);
        }
    }
}
