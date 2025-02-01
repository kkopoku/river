using River.TransactionProcessingService.Models;
using River.TransactionProcessingService.DTOs.Transfer;

namespace River.TransactionProcessingService.Repositories;

public interface ITransferRepository {

    Task<Transfer?> UpdateTransferAsync(UpdateTransferDto updateTransfer);
    Task<Transfer?> FindTransferByIdAsync(string Id);

}