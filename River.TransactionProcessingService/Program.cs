using Akka.Actor;
using DotNetEnv;
using River.TransactionProcessingService;
using River.TransactionProcessingService.Consumers;
using River.TransactionProcessingService.Actors;
using River.TransactionProcessingService.Configurations;
using River.TransactionProcessingService.Services;
using River.TransactionProcessingService.Repositories;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<ITransferService, TransferService>(); 
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

builder.Configuration["MongoDB:ConnectionString"] =
    mongoConnectionString ?? throw new Exception("MONGO_CONNECTION_STRING not set.");
builder.Configuration["MongoDB:DatabaseName"] =
    mongoDatabaseName ?? throw new Exception("MONGO_DATABASE_NAME not set.");

var actorSystem = ActorSystem.Create("TransactionProcessingService");

// Resolve dependencies from DI container
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TransferProcessorActor>>();
    var transferService = scope.ServiceProvider.GetRequiredService<ITransferService>();

    // Create actors
    var messageProcessorActor = actorSystem.ActorOf(Props.Create(() => new MessageProcessorActor()), "messageProcessor");

    var transferProcessorActor = actorSystem.ActorOf(
        Props.Create(() => new TransferProcessorActor(logger, transferService)), 
        "transferProcessor"
    );

    var transactionConsumer = new TransactionConsumer(messageProcessorActor, transferProcessorActor);

    builder.Services.AddSingleton(actorSystem);
    builder.Services.AddSingleton(messageProcessorActor);
    builder.Services.AddSingleton(transferProcessorActor);
    builder.Services.AddSingleton(transactionConsumer);
}

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

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