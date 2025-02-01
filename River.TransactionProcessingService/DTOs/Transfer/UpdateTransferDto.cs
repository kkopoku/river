using River.TransactionProcessingService.Models;

namespace River.TransactionProcessingService.DTOs.Transfer;

public class UpdateTransferDto()
{
    public required string TransactionId { get; set; }
    public bool? IsReversed { get; set; }
    public TransferStatus? Status { get; set; }

}