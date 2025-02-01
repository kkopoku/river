using River.TransactionProcessingService.DTOs.Wallet;
using River.TransactionProcessingService.Models;

namespace River.TransactionProcessingService.Repositories;

public interface IWalletRepository {

    Task<Wallet?> UpdateWalletAsync(UpdateWalletDto wallet);

    Task<Wallet> FindOneWalletAsync(string key, string value);

}