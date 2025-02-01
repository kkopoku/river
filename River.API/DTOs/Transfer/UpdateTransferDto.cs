using River.API.Models;

namespace River.API.DTOs.Transfer;

public class UpdateTransferDto {

    public required string TransactionId { get; set; }
    public bool? IsReversed { get; set; }
    public TransferStatus? Status { get; set; }

}