using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{
    public class Campaign : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("JenisPekerjaan")]
        public string? JenisPekerjaan { get; set; }

        [BsonElement("HargaPekerjaan")]
        public float? HargaPekerjaan { get; set; }

        [BsonElement("TipeKonten")]
        public string? TipeKonten { get; set; }

        [BsonElement("ReferensiVisual")]
        public string? ReferensiVisual { get; set; }

        [BsonElement("ArahanKonten")]
        public string? ArahanKonten { get; set; }

        [BsonElement("ArahanCaption")]
        public string? ArahanCaption { get; set; }

        [BsonElement("Catatan")]
        public string? Catatan { get; set; }

        [BsonElement("Website")]
        public string? Website { get; set; }

        [BsonElement("Hashtag")]
        public string? Hashtag { get; set; }

        [BsonElement("MentionAccount")]
        public string? MentionAccount { get; set; }

        [BsonElement("NamaProyek")]
        public string? NamaProyek { get; set; }

        [BsonElement("TipeProyek")]
        public string? TipeProyek { get; set; }

        [BsonElement("CoverProyek")]
        public string? CoverProyek { get; set; }
        
    }
}