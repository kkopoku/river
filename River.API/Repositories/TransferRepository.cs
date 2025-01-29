using River.API.Models;
using River.API.Configurations;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace River.API.Repositories;


public class TransferRepository(
    MongoDbContext mongoDbContext,
    ILogger<TransferRepository> logger
) : ITransferRepository
{

    private readonly IMongoCollection<Transfer> _transfers = mongoDbContext.Transfers;
    private readonly ILogger<TransferRepository> _logger = logger;

    public async Task<Transfer> CreateTransferAsync(Transfer transfer)
    {
        string tag = "[TransferRepository][CreateTransferAsync]";
        try
        {
            await _transfers.InsertOneAsync(transfer);
            return transfer;
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error occurred creating transfer");
            throw new Exception(e.Message);
        }
    }


    public async Task<List<Transfer>> GetAllTransfersAsync(int pageNumber, int pageSize){
        string tag = "[TransferRepository][GetAllTransfersAsync]";
        try
        {
            var transfers = await _transfers
                .Find(wallet => true)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return transfers;
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error getting all transfers from database");
            throw new Exception(e.Message);
        }
    }

}