using Akka.Actor;
using River.TransactionProcessingService.Services;
using River.TransactionProcessingService.Models;
using Newtonsoft.Json;


namespace River.TransactionProcessingService.Actors;


public class TransferProcessorActor : ReceiveActor
{

    private readonly ILogger<TransferProcessorActor> _logger;
    private readonly ITransferService _transferService;

    public TransferProcessorActor(ILogger<TransferProcessorActor> logger, ITransferService transferService)
    {
        _logger = logger;
        _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));;
        Receive<string>(message => HandleMessage(message));
    }


    private async void HandleMessage(string message)
    {
        string tag = $"[TransferProcessorActor][HandleMessage]";
        try
        {
            var transfer = JsonConvert.DeserializeObject<Transfer>(message);
            _logger.LogInformation($"{tag} Transfer event received, Message: {message}");

            await _transferService.ProcessTransfer(transfer);

            _logger.LogInformation("Transaction processing completed.");
        }
        catch (Exception e)
        {
            _logger.LogError($"{tag} Error processing transaction: {e.Message}");
        }


    }

}