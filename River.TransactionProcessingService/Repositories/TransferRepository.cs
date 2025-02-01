using River.TransactionProcessingService.Models;
using River.TransactionProcessingService.Configurations;
using MongoDB.Driver;
using River.TransactionProcessingService.DTOs.Transfer;


namespace River.TransactionProcessingService.Repositories;


public class TransferRepository(
    MongoDbContext mongoDbContext,
    ILogger<TransferRepository> logger
) : ITransferRepository
{

    private readonly IMongoCollection<Transfer> _transfers = mongoDbContext.Transfers;
    private readonly ILogger<TransferRepository> _logger = logger;


    public async Task<Transfer?> FindTransferByIdAsync(string id)
    {
        string tag = "[TransferRepository][FindTransferByIdAsync]";
        try
        {
            var transfer = await _transfers.Find(t => t.Id == id).FirstOrDefaultAsync();
            return transfer;
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error finding transfer by id");
            throw new Exception(e.Message);
        }
    }


    public async Task<Transfer?> UpdateTransferAsync(UpdateTransferDto updateTransfer)
    {
        string tag = "[TransferRepository][UpdateTransferAsync]";
        try
        {
            // Create the filter based on AccountNumber
            var filter = Builders<Transfer>.Filter.Eq(w => w.Id, updateTransfer.TransactionId);

            // Create a list of update definitions
            var updateDefinitions = new List<UpdateDefinition<Transfer>>();

            // Conditionally add updates for fields that are non-null and non-empty
            if (updateTransfer.IsReversed != null)
            {
                updateDefinitions.Add(Builders<Transfer>.Update.Set(w => w.IsReversed, updateTransfer.IsReversed));
            }

            if (updateTransfer.Status != null)
            {
                updateDefinitions.Add(Builders<Transfer>.Update.Set(w => w.Status, updateTransfer.Status));
            }

            // If no fields to update, return early with the existing wallet
            if (updateDefinitions.Count == 0)
            {
                return null;
            }

            // Combine the list of update definitions into a single update
            var update = Builders<Transfer>.Update.Combine(updateDefinitions);

            // Perform the update operation
            var updatedTransfer = await _transfers.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Transfer>
            {
                ReturnDocument = ReturnDocument.After
            });

            return updatedTransfer;

        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error updating transfer");
            throw new Exception(e.Message);
        }
    }

}