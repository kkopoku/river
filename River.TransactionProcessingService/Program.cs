using Akka.Actor;
using DotNetEnv;
using River.TransactionProcessingService;
using River.TransactionProcessingService.Consumers;
using River.TransactionProcessingService.Actors;
using River.TransactionProcessingService.Configurations;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();


var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");


builder.Configuration["MongoDB:ConnectionString"] =
    mongoConnectionString ?? throw new Exception("MONGO_CONNECTION_STRING not set.");
builder.Configuration["MongoDB:DatabaseName"] =
    mongoDatabaseName ?? throw new Exception("MONGO_DATABASE_NAME not set.");


var actorSystem = ActorSystem.Create("TransactionProcessingService");
var messageProcessorActor = actorSystem.ActorOf(Props.Create(() => new MessageProcessorActor()), "messageProcessor");


var transactionConsumer = new TransactionConsumer(messageProcessorActor);

builder.Services.AddSingleton(actorSystem);
builder.Services.AddSingleton(actorSystem.ActorOf<MessageProcessorActor>("transactionConsumer"));


builder.Services.AddSingleton<TransactionConsumer>();
builder.Services.AddSingleton<MessageProcessorActor>();


// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();


builder.Services.AddSingleton(actorSystem);

builder.Services.AddHostedService<Worker>();


var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    try
    {
        if (mongoDbContext.TestConnection())
        {
            Console.WriteLine("MongoDB connection successful.");
        }
        else
        {
            Console.WriteLine("MongoDB connection failed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error testing MongoDB connection: {ex.Message}");
    }
}

host.Run();
