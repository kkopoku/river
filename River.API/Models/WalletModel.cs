using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace River.API.Models;

public class Wallet {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    
    public required string AccountNumber { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]

    public required string CorridorId { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]

    public required string Id { get; set; }

    public required string OrganizationId { get; set; }

    public decimal Balance { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string City { get; set; }

    public required string PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

}