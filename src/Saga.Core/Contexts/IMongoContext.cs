using MongoDB.Driver;

namespace Saga.Core.Contexts;

public interface IMongoContext
{
    IMongoDatabase Database { get; }

    IMongoCollection<T> GetCollection<T>(string collectionName);

    IMongoClient Client { get; }
}

