

using Newtonsoft.Json;
using River.API.DTOs;
using River.API.Models;
using River.API.Repositories;


namespace River.API.Services;


public class WalletService(
    IWalletRepository walletRepository
) : IWalletServices
{

    private readonly IWalletRepository _walletRepository = walletRepository;

    public async Task<ApiResponse<Wallet>> AddWalletAsync(CreateWalletDto createWalletDto)
    {
        var wallet = new Wallet
        {
            AccountNumber = createWalletDto.AccountNumber,
            FirstName = createWalletDto.FirstName,
            LastName = createWalletDto.LastName,
            Email = createWalletDto.Email,
            PhoneNumber = createWalletDto.PhoneNumber,
            Cap = createWalletDto.Cap ?? 0
        };


        var createdWallet = await _walletRepository.CreateWalletAsync(wallet);
        return new ApiResponse<Wallet>(
        code: "200",
        message: "Wallet created successfully",
        data: createdWallet);

    }


    public async Task<ApiResponse<Wallet>> UpdateWalletAsync(UpdateWalletDto updateWalletDto)
    {
        string jsonString = JsonConvert.SerializeObject(updateWalletDto);
        Console.WriteLine("Wallet updated DTO: " + jsonString);

        // Log the result of the update operation
        var updatedWallet = await _walletRepository.UpdateWalletAsync(updateWalletDto);

        if (updatedWallet == null)
        {
            Console.WriteLine("Failed to update the wallet.");
            return new ApiResponse<Wallet>(
                code: "500",
                message: "Failed to update wallet.",
                data: null
            );
        }

        Console.WriteLine("Wallet updated successfully.");
        return new ApiResponse<Wallet>(
            code: "200",
            message: "Wallet updated successfully",
            data: updatedWallet
        );
    }



    public async Task<ApiResponse<List<Wallet>>> GetAllWalletsAsync(int pageNumber, int pageSize)
    {
        var wallets = await _walletRepository.GetAllWalletsAsync(pageNumber, pageSize);
        return new ApiResponse<List<Wallet>>(
            code: $"{200}",
            message: "Wallets fetched successfully",
            data: wallets
        );
    }

}

