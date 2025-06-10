using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace impact.Shared.Models
{
    public class Arisan : BaseModel
    {
         [BsonId]
        public string? Id { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonElement("Keterangan")]
        public string? Keterangan { get; set; }

        [BsonElement("Banner")]
        public List<string>? Banner { get; set; }

        [BsonElement("Document")]
        public List<string>? Document { get; set; }

        [BsonElement("Location")]
        public string? Location { get; set; }

        [BsonElement("TargetLot")]
        public float? TargetLot { get; set; }

        [BsonElement("TargetAmount")]
        public float? TargetAmount { get; set; }

        [BsonElement("PenagihanDate")]
        public string? PenagihanDate { get; set; }

        [BsonElement("IsAvailable")]
        public bool IsAvailable { get; set; } = true;

        [BsonElement("MemberArisans")]
        public List<MemberArisan>? MemberArisans { get; set; }
    }

    public class MemberArisan
    {
        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [BsonElement("JumlahLot")]
        public float? JumlahLot { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("IsPayed")]
        public bool IsPayed { get; set; } = false;
    }
}
