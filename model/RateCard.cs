using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class RateCard
    {
        [BsonId]
        public string? Id { get; set; }

        public string IdUser { get; set; }

        public string Currency { get; set; }

        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
