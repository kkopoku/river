using River.TransactionProcessingService.Models;

namespace River.TransactionProcessingService.Services;


public interface ITransferService{

    Task ProcessTransfer(Transfer transfer);
    Task ProcessTransferReversal(Transfer transfer);
}