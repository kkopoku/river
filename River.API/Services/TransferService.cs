using River.API.DTOs;
using River.API.DTOs.Transfer;
using River.API.Models;
using River.API.Repositories;
using Newtonsoft.Json;

namespace River.API.Services
{
    public class TransferService(
        ITransferRepository transferRepository,
        IWalletRepository walletRepository,
        IWalletServices walletServices,
        ILogger<TransferService> logger,
        IKafkaProducer kafkaProducer
        ) : ITransferService
    {
        private readonly ITransferRepository _transferRepository = transferRepository;
        private readonly ILogger<TransferService> _logger = logger;
        private readonly IWalletRepository _walletRepository = walletRepository;
        private readonly IWalletServices _walletService = walletServices;
        private readonly IKafkaProducer _kafkaProducer = kafkaProducer;

        public async Task<ApiResponse<string>> CreateTransferAsync(CreateTransferDto createTransferDto)
        {
            string tag = "[TransferService][CreateTransferAsync]";
            try
            {
                var transfer = new Transfer
                {
                    From = createTransferDto.FromAccountNumber,
                    To = createTransferDto.ToAccountNumber,
                    Amount = createTransferDto.Amount,
                    Status = TransferStatus.Success
                };

                var createdTransfer = await _transferRepository.CreateTransferAsync(transfer);

                // send event to process transfer
                await _kafkaProducer.ProduceAsync(
                        "river_transactions",
                        "transfer",
                        JsonConvert.SerializeObject(createdTransfer)
                    );
                _logger.LogInformation($"sent event to process transfer: {createdTransfer.Id} ");

                return new ApiResponse<string>(
                    code: "200",
                    message: "Transfer is sent for processing",
                    data: createdTransfer.Id
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"{tag} Error creating transfer: {ex.Message}", ex);
                return new ApiResponse<string>(
                    code: "500",
                    message: ex.Message ?? "An error occurred while creating the transfer",
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


        public async Task<ApiResponse<string>> ReverseTransferAsync(ReverseTransferDto reverseTransferDto)
        {
            string tag = "[TransferService.cs][ReverseTransferAsync]";
            try
            {
                var foundTransfer = await _transferRepository.FindTransferByIdAsync(reverseTransferDto.TransactionId);

                if (foundTransfer == null)
                {
                    return new ApiResponse<string>(
                        code: "404",
                        message: "Transfer not found",
                        data: null
                    );
                }

                if (foundTransfer.IsReversed)
                {
                    return new ApiResponse<string>(
                        code: "400",
                        message: "Transfer has already been reversed",
                        data: null
                    );
                }


                // send event to kafka server
                await _kafkaProducer.ProduceAsync(
                        "river_transactions",
                        "reversal",
                        JsonConvert.SerializeObject(foundTransfer)
                    );
                _logger.LogInformation($"{tag} sent event to process reversal: {foundTransfer.Id} ");

                
                return new ApiResponse<string> (
                    code: "200",
                    message: "Transfer reversal is processing",
                    data: foundTransfer.Id
                );
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"{tag} Error creating transfer: {ex.Message}", ex);
                return new ApiResponse<string>(
                    code: "500",
                    message: ex.Message ?? "An error occurred while creating the transfer",
                    data: null);
            }
        }



        private async Task<DebitResultDto> Debit(Transfer transfer)
        {
            string tag = "[TransferService][Debit]";
            _logger.LogInformation($"{tag} From Account Number: {transfer.From}");
            _logger.LogInformation($"{tag} To Account Number: {transfer.To}");

            var from = await _walletRepository.FindOneWalletAsync("AccountNumber", transfer.From);
            var to = await _walletRepository.FindOneWalletAsync("AccountNumber", transfer.To);

            if (from == null || to == null)
            {
                throw new Exception("One or both accounts do not exist");
            }

            if (from.Id == to.Id)
            {
                throw new Exception("Cannot transfer to the same account");
            }

            var cap = from.Cap;
            var fromBalanceAfter = from.Balance - transfer.Amount;
            var toBalanceAfter = to.Balance + transfer.Amount;

            if (fromBalanceAfter < cap) throw new Exception("Insufficient funds to transfer");

            UpdateWalletDto fromUpdateWalletDto = new()
            {
                AccountNumber = from.AccountNumber,
                Balance = from.Balance - transfer.Amount
            };

            UpdateWalletDto toUpdateWalletDto = new()
            {
                AccountNumber = to.AccountNumber,
                Balance = to.Balance + transfer.Amount
            };

            var fromAfter = await _walletRepository.UpdateWalletAsync(fromUpdateWalletDto);
            var toAfter = await _walletRepository.UpdateWalletAsync(toUpdateWalletDto);

            DebitResultDto debitResult = new()
            {
                FromWallet = fromAfter,
                ToWallet = toAfter
            };

            return debitResult;
        }

        private async Task<ReverseTransferResultDto?> Reverse(Transfer transfer)
        {

            string tag = "[TransferService.cs][Reverse]";
            _logger.LogInformation($"{tag} From Account Number: {transfer.From}");

            var from = await _walletRepository.FindOneWalletAsync("AccountNumber", transfer.From);
            var to = await _walletRepository.FindOneWalletAsync("AccountNumber", transfer.To);

            if (from == null || to == null)
            {
                throw new Exception("One or both accounts do not exist");
            }

            var fromBalanceAfter = from.Balance + transfer.Amount;
            var toBalanceAfter = to.Balance - transfer.Amount;

            UpdateWalletDto fromUpdateWalletDto = new()
            {
                AccountNumber = from.AccountNumber,
                Balance = fromBalanceAfter
            };

            UpdateWalletDto toUpdateWalletDto = new()
            {
                AccountNumber = to.AccountNumber,
                Balance = toBalanceAfter
            };

            UpdateTransferDto reversedTransfer = new() {
                TransactionId = transfer.Id,
                IsReversed = true
            };

            var fromAfter = await _walletRepository.UpdateWalletAsync(fromUpdateWalletDto);
            var toAfter = await _walletRepository.UpdateWalletAsync(toUpdateWalletDto);
            var transferAfter = await _transferRepository.UpdateTransferAsync(reversedTransfer);
            

            ReverseTransferResultDto result = new() {
                From = fromAfter,
                To = toAfter,
                Transfer = transferAfter
            };

            return result;
        }
    }
}
