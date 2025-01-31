using Confluent.Kafka;
using Akka.Actor;

namespace River.TransactionProcessingService.Consumers
{
    public class TransactionConsumer : IHostedService
    {
        // private readonly ILogger<TransactionConsumer> _logger;
        private readonly IConsumer<string, string> _consumer;
        private Boolean disposed = false;
        private readonly string _topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? throw new Exception("KAFKA_TOPIC not set.");
        private readonly IActorRef _messageProcessorActor;
        private CancellationTokenSource _cancellationTokenSource;


        public TransactionConsumer(IActorRef messageProcessorActor)
        {
            _messageProcessorActor = messageProcessorActor;

            var config = new ConsumerConfig
            {
                BootstrapServers =
                Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAPSERVERS")
                ?? throw new Exception("KAFKA_BOOTSTRAPSERVERS not set."),
                SaslUsername =
                Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME")
                ?? throw new Exception("KAFKA_SASL_USERNAME not set."),
                SaslPassword =
                Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD")
                ?? throw new Exception("KAFKA_SASL_PASSWORD not set."),
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                GroupId = "river",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken
            );

            Task.Run(() => StartConsuming(_cancellationTokenSource.Token), cancellationToken);

            Console.WriteLine("Transaction Event started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping Transaction Event...");
            _cancellationTokenSource.Cancel();
            _consumer.Close();
            return Task.CompletedTask;
        }

        public void StartConsuming(CancellationToken cancellationToken)
        {
            string tag = "[TransactionConsumer.cs][StartConsuming]";
            _consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        Console.WriteLine($"{tag} Transaction message received: {consumeResult.Message.Value}");

                        // Handle the transaction event here
                        _messageProcessorActor.Tell(consumeResult.Message.Value);
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"{tag} Error consuming message: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{tag} Error processing message: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{tag} Consumer loop cancelled.");
            }
            finally
            {
                _consumer.Close();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Console.WriteLine("Closing Kafka consumer.");
                _consumer.Dispose();
                disposed = true;
            }
        }
    }
}
