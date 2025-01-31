using Akka.Actor;


namespace River.TransactionProcessingService.Actors;


public class MessageProcessorActor : ReceiveActor
{

    // private readonly ILogger<MessagingProcessingActor> _logger;

    public MessageProcessorActor()
    {
        Receive<string>(message => HandleMessage(message));
    }


    private void HandleMessage(string message)
    {
        Console.WriteLine($"Processing transaction event in Akka Actor: {message}");

        // Simulate transaction processing
        Thread.Sleep(4000);

        Console.WriteLine("Transaction processing completed.");
    }

}