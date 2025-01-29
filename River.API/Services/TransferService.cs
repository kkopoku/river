using River.API.DTOs;
using River.API.Models;
using River.API.Repositories;

namespace River.API.Services
{
    public class TransferService(
        ITransferRepository transferRepository,
        IWalletRepository walletRepository,
        IWalletServices walletServices,
        ILogger<TransferService> logger) : ITransferService
    {
        private readonly ITransferRepository _transferRepository = transferRepository;
        private readonly ILogger<TransferService> _logger = logger;
        private readonly IWalletRepository _walletRepository = walletRepository;
        private readonly IWalletServices _walletService = walletServices;

        public async Task<ApiResponse<Transfer>> CreateTransferAsync(CreateTransferDto createTransferDto)
        {
            string tag = "[TransferService][CreateTransferAsync]";
            try
            {
                var transfer = new Transfer
                {
                    From = createTransferDto.FromAccountNumber,
                    To = createTransferDto.ToAccountNumber,
                    Amount = createTransferDto.Amount,
                };

                _logger.LogInformation($"{tag} From Account Number: {createTransferDto.FromAccountNumber}");
                _logger.LogInformation($"{tag} To Account Number: {createTransferDto.ToAccountNumber}");

                var from = await _walletRepository.FindOneWalletAsync("AccountNumber", createTransferDto.FromAccountNumber);
                var to = await _walletRepository.FindOneWalletAsync("AccountNumber", createTransferDto.ToAccountNumber);

                if (from == null || to == null)
                {
                    return new ApiResponse<Transfer>(
                        code: "400",
                        message: "One or both accounts do not exist",
                        data: null
                    );
                }

                UpdateWalletDto fromUpdateWalletDto= new UpdateWalletDto
                {
                    AccountNumber = from.AccountNumber,
                    Balance = from.Balance - transfer.Amount
                };

                UpdateWalletDto toUpdateWalletDto= new UpdateWalletDto
                {
                    AccountNumber = to.AccountNumber,
                    Balance = to.Balance + transfer.Amount
                };

                await _walletService.UpdateWalletAsync(fromUpdateWalletDto);
                await _walletService.UpdateWalletAsync(toUpdateWalletDto);

                var createdTransfer = await _transferRepository.CreateTransferAsync(transfer);

                return new ApiResponse<Transfer>(
                    code: "201",
                    message: "Transfer created successfully",
                    data: createdTransfer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating transfer: {ex.Message}", ex);
                return new ApiResponse<Transfer>(
                    code: "500",
                    message: "An error occurred while creating the transfer",
                    data: null);
            }
        }

        public async Task<ApiResponse<List<Transfer>>> GetAllTransfersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var transfers = await _transferRepository.GetAllTransfersAsync(pageNumber, pageSize);

                return new ApiResponse<List<Transfer>>(
                    code: "200",
                    message: "Transfers fetched successfully",
                    data: transfers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching transfers: {ex.Message}", ex);
                return new ApiResponse<List<Transfer>>(
                    code: "500",
                    message: "An error occurred while fetching transfers",
                    data: null);
            }
        }
    }
}
