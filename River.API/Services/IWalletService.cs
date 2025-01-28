using River.API.DTOs;
using River.API.Models;

namespace River.API.Services
{
    public interface IWalletServices
    {
        Task<ApiResponse<Wallet>> AddWalletAsync(CreateWalletDto createWalletDto);
    }
}
