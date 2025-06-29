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