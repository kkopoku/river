using DotNetEnv;
using River.API.Configurations;

var builder = WebApplication.CreateBuilder(args);
Env.Load();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

builder.Configuration["MongoDB:ConnectionString"] =
    mongoConnectionString ?? throw new Exception("MONGO_CONNECTION_STRING not set.");
builder.Configuration["MongoDB:DatabaseName"] =
    mongoDatabaseName ?? throw new Exception("MONGO_DATABASE_NAME not set.");


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();