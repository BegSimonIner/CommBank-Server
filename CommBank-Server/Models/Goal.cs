using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommBank.Models;

public class Goal
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]  
    public string? Name { get; set; }

    [BsonElement("Icon")] 
    public string? Icon { get; set; }

    [BsonElement("TargetAmount")]
    public UInt64 TargetAmount { get; set; } = 0;

    [BsonElement("TargetDate")]
    public DateTime TargetDate { get; set; }

    [BsonElement("Balance")]
    public double Balance { get; set; } = 0.00;

    [BsonElement("Created")]
    public DateTime Created { get; set; } = DateTime.Now;

    [BsonElement("TransactionIds")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? TransactionIds { get; set; }

    [BsonElement("TagIds")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? TagIds { get; set; }

    [BsonElement("UserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }
}