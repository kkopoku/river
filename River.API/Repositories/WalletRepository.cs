using MongoDB.Driver;
using River.API.Configurations;
using River.API.DTOs;
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
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new Exception("AccountNumber must be unique.");
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error occurred creating wallet");
            throw new Exception(e.Message);
        }
    }


    public async Task<List<Wallet>> GetAllWalletsAsync(int pageNumber, int pageSize)
    {
        string tag = "[WalletRepository][GetAllWalletsAsync]";
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


    public async Task<Wallet> FindOneWalletByEmailAsync(string email)
    {
        return await _wallets.Find(Builders<Wallet>.Filter.Eq(w => w.Email, email)).FirstOrDefaultAsync();
    }


    public async Task<Wallet> FindOneWalletByIdAsync(string id)
    {
        return await _wallets.Find(Builders<Wallet>.Filter.Eq(w => w.Id, id)).FirstOrDefaultAsync();
    }

    public async Task<Wallet> FindOneWalletAsync(string key, string value)
    {
        var filter = Builders<Wallet>.Filter.Eq(key, value);
        return await _wallets.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Wallet> UpdateWalletAsync(UpdateWalletDto updateWalletDto)
    {
        var filter = Builders<Wallet>.Filter.Eq(w => w.AccountNumber, updateWalletDto.AccountNumber);

        // Create the update definition
        var update = Builders<Wallet>.Update
            .Set(w => w.FirstName, updateWalletDto.FirstName)
            .Set(w => w.LastName, updateWalletDto.LastName)
            .Set(w => w.Email, updateWalletDto.Email)
            .Set(w => w.PhoneNumber, updateWalletDto.PhoneNumber);

        // Perform the update operation
        var updatedWallet = await _wallets.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Wallet>
        {
            ReturnDocument = ReturnDocument.After // Returns the updated document
        });


        return updatedWallet;
    }
}