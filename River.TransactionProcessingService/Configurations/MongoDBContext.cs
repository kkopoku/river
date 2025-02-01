using Microsoft.Extensions.Options;
using MongoDB.Driver;
using River.TransactionProcessingService.Models;

namespace River.TransactionProcessingService.Configurations
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            ConfigureIndexes();
        }

        public IMongoCollection<Wallet> Wallets => _database.GetCollection<Wallet>("Wallets");
        public IMongoCollection<Transfer> Transfers => _database.GetCollection<Transfer>("Transfers");


        private void ConfigureIndexes()
        {
            // Create unique index on AccountNumber for Wallets collection
            var walletsIndexKeys = Builders<Wallet>.IndexKeys.Ascending(w => w.AccountNumber);
            var uniqueIndexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Wallet>(walletsIndexKeys, uniqueIndexOptions);
            Wallets.Indexes.CreateOne(indexModel);
        }


        public bool TestConnection()
        {
            try
            {
                // List collections as a test query
                _database.ListCollections();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
