using Akka.Actor;

namespace River.TransactionProcessingService.Actors;

public class ActorProvider
{
    private readonly ActorSystem _actorSystem;
    private readonly IActorRef _inventoryActor;
    private readonly HttpClient _httpClient;

    public ActorProvider(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;

        // Create InventoryActor when ActorProvider is initialized
        _inventoryActor = _actorSystem.ActorOf(
            Props.Create(() => new MessageProcessorActor()),
            "messageActor"
        );
    }

    // public IActorRef GetOrderManagerActor()
    // {
    //     return _actorSystem.ActorOf(
    //         Props.Create(() => new OrderManagerActor(_inventoryActor, _httpClient)),
    //         "orderManagerActor"
    //     );
    // }
}
