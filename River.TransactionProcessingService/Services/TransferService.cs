
using River.TransactionProcessingService.Models;
using River.TransactionProcessingService.Repositories;
using River.TransactionProcessingService.DTOs.Transfer;
using River.TransactionProcessingService.DTOs.Wallet;
using Newtonsoft.Json;

namespace River.TransactionProcessingService.Services
{
    public class TransferService(
        ITransferRepository transferRepository,
        IWalletRepository walletRepository,
        ILogger<TransferService> logger
        ) : ITransferService
    {
        private readonly ITransferRepository _transferRepository = transferRepository;
        private readonly ILogger<TransferService> _logger = logger;
        private readonly IWalletRepository _walletRepository = walletRepository;

        public async Task ProcessTransfer(Transfer transfer)

        {
            string tag = $"[ProcessTransfer.cs][ProcessTransfer][{transfer.Id}]";
            try
            {
                _logger.LogInformation($"About to process Transfer with Id: {transfer.Id} ");

                DebitResultDto debitResponse = await Debit(transfer);

                UpdateTransferDto _transfer = new (){
                    TransactionId = transfer.Id,
                    Status = TransferStatus.Success
                };

                var completedTransfer = await _transferRepository.UpdateTransferAsync(_transfer);

                string jsonTransfer = JsonConvert.SerializeObject(completedTransfer);
                _logger.LogInformation($"Transfer is completed: {jsonTransfer}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{tag} Transfer failed ... Error: {ex.Message}");
                _logger.LogError($"{tag} Updating to failed ... Error: {ex.Message}");
                UpdateTransferDto _transfer = new (){
                    TransactionId = transfer.Id,
                    Status = TransferStatus.Failed
                };
                await _transferRepository.UpdateTransferAsync(_transfer);
            }
        }



        public async Task ReverseTransferAsync(Transfer transfer)
        {
            string tag = $"[TransferService.cs][ReverseTransferAsync][{transfer.Id}]";
            try
            {
                var foundTransaction = await _transferRepository.FindTransferByIdAsync(transfer.Id) ?? throw new Exception($"{tag} Transfer record not found");
                
                if (foundTransaction.IsReversed) throw new Exception($"{tag} Transfer has already been reversed");

                var result = await Reverse(foundTransaction);
                
                _logger.LogInformation($"{tag} Transfer reversal completed successfully");
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"{tag} Error reversing transfer. Error: {ex.Message}");
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
