using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace impact.Shared.Models
{

    /// <summary>
    /// 1. Arisan
    /// 2. Patungan
    /// 3. Sedekah
    /// 4. Koperasi
    /// 5. TopUp
    /// </summary>
    /// 
    /// 1. Expenses
    /// 2. Income
    public class Transaksi : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }
        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("IdTransaksi")]
        public string? IdTransaksi { get; set; }

        [BsonElement("Type")]
        public string? Type { get; set; }

        [BsonElement("Nominal")]
        public float? Nominal { get; set; }

        [BsonElement("Ket")]
        public string? Ket { get; set; }

        [BsonElement("Status")]
        public string? Status { get; set; }
    }
}