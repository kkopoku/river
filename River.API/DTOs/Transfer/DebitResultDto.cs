using River.API.Models;
using River.API.DTOs.Wallet;

namespace River.API.DTOs.Transfer;

public class DebitResultDto
{
    public required WalletDto FromWallet { get; set; }
    public required WalletDto ToWallet { get; set; }
}