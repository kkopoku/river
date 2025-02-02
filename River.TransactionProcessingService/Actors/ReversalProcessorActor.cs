using Akka.Actor;
using River.TransactionProcessingService.Models;
using Newtonsoft.Json;
using River.TransactionProcessingService.Services;


namespace River.TransactionProcessingService.Actors;


public class ReversalProcessorActor : ReceiveActor {

    private readonly ILogger<ReversalProcessorActor> _logger;
    private readonly ITransferService _transferService;
    
    public ReversalProcessorActor(ILogger<ReversalProcessorActor> logger, ITransferService transferService){
        _logger = logger;
        _transferService = transferService;
        Receive<string>(HandleMessage);
    }


    public async void HandleMessage(string message){
        string tag = "[ReversalProcessorActor.cs][HandleMessage]";

        var transfer = JsonConvert.DeserializeObject<Transfer>(message);
        _logger.LogInformation($"{tag} Transfer reversal event received for {transfer.Id}, Message: {message}");

        await _transferService.ProcessTransferReversal(transfer);

        Console.WriteLine($"{tag}[{transfer.Id}] Reversal completed.");
    }

}