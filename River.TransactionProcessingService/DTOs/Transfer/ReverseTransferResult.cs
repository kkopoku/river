

namespace River.TransactionProcessingService.DTOs.Transfer;


public class ReverseTransferResultDto {

    public required Models.Wallet From { get; set; }
    public required Models.Wallet To { get; set; }
    public required Models.Transfer Transfer { get; set; }

}