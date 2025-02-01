using River.TransactionProcessingService.Models;

namespace River.TransactionProcessingService.Services;


public interface ITransferService{

    Task ProcessTransfer(Transfer transfer);
    Task ReverseTransferAsync(Transfer transfer);
}