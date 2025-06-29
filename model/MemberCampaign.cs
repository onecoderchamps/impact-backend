using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class MemberCampaign : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("IdCampaign")]
        public string? IdCampaign { get; set; }

        [BsonElement("Status")]
        public bool? Status { get; set; }
        [BsonElement("InviteBy")]
        public string? InviteBy { get; set; }
        
    }
}