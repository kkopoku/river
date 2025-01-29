using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace River.API.Models;

public class Transfer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string From { get; set; }

    public required string To { get; set; }

    // [BsonRepresentation(BsonType.ObjectId)]
    // public required string OrganisationId { get; set; }

    // [BsonRepresentation(BsonType.ObjectId)]
    // public required string CorridorId { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

    public decimal Amount { get; set; }
}