public class CreateCampaignDto
{
        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? JenisPekerjaan { get; set; }

        public float? HargaPekerjaan { get; set; }

        public string? TipeKonten { get; set; }

        public string? ReferensiVisual { get; set; }

        public string? ArahanKonten { get; set; }

        public string? ArahanCaption { get; set; }

        public string? Catatan { get; set; }

        public string? Website { get; set; }

        public string? Hashtag { get; set; }

        public string? MentionAccount { get; set; }

        public string? NamaProyek { get; set; }

        public string? TipeProyek { get; set; }

        public string? CoverProyek { get; set; }
}

public class RegisterCampaignDto
{
        public string? IdCampaign { get; set; }
        public string? IdUser { get; set; }

}

public class UpdateCampaignDto
{
        public string? IdCampaign { get; set; }
        public string? IdUser { get; set; }
        public string? Status { get; set; }

}

public class PayCampaignDto
{
        public string? IdCampaign { get; set; }
        public int? HargaPekerjaan { get; set; }

}

public class PayCallbackCampaignDto
{
        public string? merchantOrderId { get; set; }
        public int? amount { get; set; }

}

public class ItemDetail
{
        public string name { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
}

public class BillingAddress
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string address { get; set; }
    public string city { get; set; }
    public string postalCode { get; set; }
    public string phone { get; set; }
    public string countryCode { get; set; }
}

public class CustomerDetail
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public string phoneNumber { get; set; }
    public BillingAddress billingAddress { get; set; }
    public BillingAddress shippingAddress { get; set; }
}