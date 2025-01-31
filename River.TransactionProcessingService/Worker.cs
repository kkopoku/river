using River.TransactionProcessingService.Actors;
using Confluent.Kafka;
using Akka.Actor;
using River.TransactionProcessingService.Consumers;

namespace River.TransactionProcessingService;

public class Worker : BackgroundService
{

    private readonly ILogger<Worker> _logger;
    // private readonly ActorSystem _actorSystem;
    private readonly TransactionConsumer _transactionConsumer;

    public Worker(ILogger<Worker> logger, TransactionConsumer transactionConsumer)
    {
        _logger = logger;
        // _actorSystem = ActorSystem.Create("TransactionProcessingSystem");
        _transactionConsumer = transactionConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     if (_logger.IsEnabled(LogLevel.Information))
        //     {
        //         _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     }
        //     await Task.Delay(1000, stoppingToken);
        // }

        _logger.LogInformation("Transaction Processing Service started.");

        var transactionTask = Task.Run(() => _transactionConsumer.StartConsuming(stoppingToken), stoppingToken);
        await Task.WhenAll(transactionTask);
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {

        _logger.LogInformation("Transaction Processing Service stopping.");
        return base.StopAsync(cancellationToken);
    }
}
