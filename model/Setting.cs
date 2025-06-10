using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class Setting : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Key")]
        public string? Key { get; set; }
        [BsonElement("Value")]
        public string? Value { get; set; }
    }

    public class Setting2 : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Key")]
        public string? Key { get; set; }
        [BsonElement("Value")]
        public List<string>? Value { get; set; }
    }

    public class Setting3 : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Key")]
        public string? Key { get; set; }
        [BsonElement("Value")]
        public float? Value { get; set; }
    }
}