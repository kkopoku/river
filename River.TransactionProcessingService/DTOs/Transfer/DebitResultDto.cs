namespace River.TransactionProcessingService.DTOs.Transfer;

public class DebitResultDto
{
    public required Models.Wallet FromWallet { get; set; }
    public required Models.Wallet ToWallet { get; set; }
}