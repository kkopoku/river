using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace River.API.Models;

public class Wallet
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    // [BsonRepresentation(BsonType.ObjectId)]

    // public required string CorridorId { get; set; }
    // [BsonRepresentation(BsonType.ObjectId)]

    // public required string OrganizationId { get; set; }

    public required string AccountNumber { get; set; }

    public decimal Balance { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

}