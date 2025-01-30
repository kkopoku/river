using Confluent.Kafka;
using River.API.Configurations;
using Microsoft.Extensions.Options;


namespace River.API.Services;


public interface IKafkaProducer {
    Task ProduceAsync(string topic, string key, string message);
}


public class KafkaProducer : IKafkaProducer {

    private readonly IProducer<string, string> _producer;
    private readonly string _topic;


    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings){

        var settings = kafkaSettings.Value;

        var config = new ProducerConfig{
            BootstrapServers = settings.BootstrapServers,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _topic = settings.Topic;
    }

    

    public async Task ProduceAsync(string topic, string key, string message)
    {
        try
        {
            var result = await _producer.ProduceAsync(
                topic ?? _topic,
                new Message<string, string> { Key = key, Value = message }
            );

            Console.WriteLine($"Message sent to {result.TopicPartitionOffset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

}