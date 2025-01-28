using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace River.API.Models;

public class Organisation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Name { get; set; }

    public required string PrimaryContact { get; set; }

    public required List<string> Whitelist { get; set; }

    public string? SecondaryContact { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }
}