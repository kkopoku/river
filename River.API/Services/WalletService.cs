

using River.API.DTOs;
using River.API.Models;
using River.API.Repositories;

namespace River.API.Services;


public class WalletService(
    IWalletRepository walletRepository
):IWalletServices
{

    private readonly IWalletRepository _walletRepository = walletRepository;

    public async Task<ApiResponse<Wallet>> AddWalletAsync(CreateWalletDto createWalletDto){
        var wallet = new Wallet
        {
            AccountNumber = createWalletDto.AccountNumber,
            FirstName = createWalletDto.FirstName,
            LastName = createWalletDto.LastName,
            Email = createWalletDto.Email,
            PhoneNumber = createWalletDto.PhoneNumber
        };

        var createdWallet = await _walletRepository.CreateWalletAsync(wallet);
        return new ApiResponse<Wallet>(
            code: $"{201}",
            message: "Wallet created successfully",
            data: createdWallet
        );
        // return createdWallet;
    }

}

