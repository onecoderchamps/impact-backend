public class CreateOrUpdateRateCardDto
{
    public string Currency { get; set; }
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
}