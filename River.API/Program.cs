using DotNetEnv;
using River.API.Configurations;
using River.API.Services;
using River.API.Repositories;

var builder = WebApplication.CreateBuilder(args);
Env.Load();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

var kafkaBootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAPSERVERS");
var kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
var kafkaSaslUsername = Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME");
var kafkaSaslPassword = Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD");


builder.Configuration["MongoDB:ConnectionString"] =
    mongoConnectionString ?? throw new Exception("MONGO_CONNECTION_STRING not set.");
builder.Configuration["MongoDB:DatabaseName"] =
    mongoDatabaseName ?? throw new Exception("MONGO_DATABASE_NAME not set.");

builder.Configuration["Kafka:BootstrapServers"] =
kafkaBootstrapServers ?? throw new Exception("KAFKA_BOOTSTRAPSERVERS not set.");
builder.Configuration["Kafka:Topic"] = kafkaTopic ?? throw new Exception("KAFKA_TOPIC not set.");
builder.Configuration["Kafka:SaslUsername"] =
    kafkaSaslUsername ?? throw new Exception("KAFKA_SASL_USERNAME not set.");
builder.Configuration["Kafka:SaslPassword"] =
    kafkaSaslPassword ?? throw new Exception("KAFKA_SASL_PASSWORD not set.");


builder.Services.AddHttpContextAccessor();

// Add services
builder.Services.AddScoped<IWalletServices, WalletService>();
builder.Services.AddScoped<ITransferService, TransferService>();

// Add repositories
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();


// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();


using (var scope = app.Services.CreateScope())
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

app.Run();