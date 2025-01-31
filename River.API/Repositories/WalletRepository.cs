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
        // Create the filter based on AccountNumber
        var filter = Builders<Wallet>.Filter.Eq(w => w.AccountNumber, updateWalletDto.AccountNumber);

        // Create a list of update definitions
        var updateDefinitions = new List<UpdateDefinition<Wallet>>();

        // Conditionally add updates for fields that are non-null and non-empty
        if (!string.IsNullOrEmpty(updateWalletDto.FirstName))
        {
            updateDefinitions.Add(Builders<Wallet>.Update.Set(w => w.FirstName, updateWalletDto.FirstName));
        }

        if (!string.IsNullOrEmpty(updateWalletDto.LastName))
        {
            updateDefinitions.Add(Builders<Wallet>.Update.Set(w => w.LastName, updateWalletDto.LastName));
        }

        if (!string.IsNullOrEmpty(updateWalletDto.Email))
        {
            updateDefinitions.Add(Builders<Wallet>.Update.Set(w => w.Email, updateWalletDto.Email));
        }

        if (updateWalletDto.Balance.HasValue)
        {
            updateDefinitions.Add(Builders<Wallet>.Update.Set(w => w.Balance, updateWalletDto.Balance));
        }

        if (!string.IsNullOrEmpty(updateWalletDto.PhoneNumber))
        {
            updateDefinitions.Add(Builders<Wallet>.Update.Set(w => w.PhoneNumber, updateWalletDto.PhoneNumber));
        }

        // If no fields to update, return early with the existing wallet
        if (!updateDefinitions.Any())
        {
            return null; // No valid fields provided for update
        }

        // Combine the list of update definitions into a single update
        var update = Builders<Wallet>.Update.Combine(updateDefinitions);

        // Perform the update operation
        var updatedWallet = await _wallets.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Wallet>
        {
            ReturnDocument = ReturnDocument.After
        });

        return updatedWallet;
    }
}