using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class Attachments : BaseModelUser
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("fileName")]
        public string? fileName { get; set; }

        [BsonElement("type")]
        public string? type { get; set; }

        [BsonElement("path")]
        public string? path { get; set; }
        [BsonElement("size")]
        public long? size { get; set; }
    }

    public class BaseModelUser
    {
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
        [BsonElement("UserId")]
        public string? UserId {get; set;}
        [BsonElement("sequence")]
        public long? sequence {get; set;}
        [BsonElement("typeWidget")]
        public string? typeWidget {get; set;}
        [BsonElement("width")]
        public string? width {get; set;}
    }
}