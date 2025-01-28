using River.API.Models;

namespace River.API.Repositories
{
    public interface IWalletRepository
    {
        // Task<Wallet> GetWalletByIdAsync(string id);
        // Task<Wallet> GetWalletByUserIdAsync(string userId);
        Task<Wallet> CreateWalletAsync(Wallet wallet);
        Task<List<Wallet>> GetAllWalletsAsync(int page, int pageSize);
        // Task<Wallet> UpdateWalletAsync(Wallet wallet);
        // Task<Wallet> DeleteWalletAsync(string id);
    }
}