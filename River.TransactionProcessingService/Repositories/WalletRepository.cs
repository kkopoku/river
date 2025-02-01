using River.TransactionProcessingService.Models;
using River.TransactionProcessingService.DTOs.Wallet;
using River.TransactionProcessingService.Configurations;
using MongoDB.Driver;

namespace River.TransactionProcessingService.Repositories;


public class WalletRepository (
    MongoDbContext mongoDbContext
) : IWalletRepository {

    private readonly IMongoCollection<Wallet> _wallets = mongoDbContext.Wallets;

    public async Task<Wallet?> UpdateWalletAsync(UpdateWalletDto updateWalletDto)
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
        if (updateDefinitions.Count == 0)
        {
            return null;
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

      public async Task<Wallet> FindOneWalletAsync(string key, string value)
    {
        var filter = Builders<Wallet>.Filter.Eq(key, value);
        return await _wallets.Find(filter).FirstOrDefaultAsync();
    }

}