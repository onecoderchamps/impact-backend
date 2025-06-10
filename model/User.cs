using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class User : BaseModel
    {
        [BsonId]
        // [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get; set;}
        
        [BsonElement("Email")]
        public string? Email {get; set;}

        [BsonElement("FullName")]
        public string? FullName {get; set;}

        [BsonElement("Phone")]
        public string? Phone {get; set;}

        [BsonElement("Image")]
        public string? Image {get; set;}

        [BsonElement("IdRole")]
        public string? IdRole {get; set;}

        [BsonElement("Balance")]
        public float? Balance {get; set;}
        [BsonElement("Address")]
        public string? Address {get; set;}
    }
}