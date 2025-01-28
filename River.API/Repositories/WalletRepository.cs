using MongoDB.Driver;
using River.API.Configurations;
using River.API.Controllers;
using River.API.Models;

namespace River.API.Repositories;


public class WalletRepository(
    MongoDbContext mongoDbContext,
    ILogger<WalletRepository> logger
    ) : IWalletRepository
{

    private readonly IMongoCollection<Wallet> _wallets = mongoDbContext.Wallets;
    private readonly ILogger<WalletRepository> _logger = logger;

    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        string tag = "[WalletRepository][CreateWalletAsync]";
        try
        {
            await _wallets.InsertOneAsync(wallet);
            return wallet;
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error occurred creating wallet");
            throw new Exception(e.Message);
        }
    }


    public async Task<List<Wallet>> GetAllWalletsAsync(int pageNumber, int pageSize)
    {
        string tag =  "[WalletRepository][GetAllWalletsAsync]";
        try
        {
            var wallets = await _wallets
                .Find(wallet => true)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return wallets;
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error getting all wallets from database");
            throw new Exception(e.Message);
        }
    }


}