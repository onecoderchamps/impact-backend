using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class Scraper : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }
        [BsonElement("IdUser")]
        public string? IdUser { get; set; }
        [BsonElement("Type")]
        public string? Type { get; set; }

        [BsonElement("Tiktok")]
        public TikTokUser? Tiktok { get; set; }

        [BsonElement("Video")]
        public List<TikTokVideo>? Video { get; set; }

        [BsonElement("Instagram")]
        public InstagramProfile? Instagram { get; set; }
        [BsonElement("Youtube")]
        public YoutubeProfile? Youtube { get; set; }

        [BsonElement("Linkedin")]
        public LinkedInProfile? Linkedin { get; set; }
        
    }
}