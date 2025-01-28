using MongoDB.Driver;
using River.API.Configurations;
using River.API.Models;

namespace River.API.Repositories;


public class WalletRepository(MongoDbContext mongoDbContext): IWalletRepository
{

    private readonly IMongoCollection<Wallet> _wallets = mongoDbContext.Wallets;

    public async Task<Wallet>CreateWalletAsync(Wallet wallet)
    {
        try
        {
            await _wallets.InsertOneAsync(wallet);
            return wallet;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


}