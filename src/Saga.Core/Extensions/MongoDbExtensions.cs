using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Saga.Core.Contexts;
using System.Net.Sockets;

namespace Saga.Core.Extensions;
public static class MongoDbExtensions
{
    public static void AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("MongoDb"));

        static void SocketConfigurator(Socket s) => s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

        settings.SocketTimeout = TimeSpan.FromMinutes(1);
        settings.ConnectTimeout = TimeSpan.FromSeconds(60);
        settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(20);
        settings.ClusterConfigurator = builder => builder
            .ConfigureTcp(tcp => tcp.With(socketConfigurator: (Action<Socket>)SocketConfigurator));

        var mongoClient = new MongoClient(settings);

        services.AddSingleton(t => (IMongoContext)new MongoContext("saga-example", mongoClient));
    }
}
