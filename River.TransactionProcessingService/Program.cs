using Akka.Actor;
using DotNetEnv;
using River.TransactionProcessingService;
using River.TransactionProcessingService.Consumers;
using River.TransactionProcessingService.Actors;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);


var actorSystem = ActorSystem.Create("TransactionProcessingService");
var messageProcessorActor = actorSystem.ActorOf(Props.Create(() => new MessageProcessorActor()), "messageProcessor");


var transactionConsumer = new TransactionConsumer(messageProcessorActor);

builder.Services.AddSingleton(actorSystem);
builder.Services.AddSingleton(actorSystem.ActorOf<MessageProcessorActor>("transactionConsumer"));


builder.Services.AddSingleton<TransactionConsumer>();
builder.Services.AddSingleton<MessageProcessorActor>();


builder.Services.AddSingleton(actorSystem);

builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
