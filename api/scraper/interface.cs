public interface IScraperService
{
    Task<Object> scraperTiktok(TikTokProfileRequest items, string idUser);
}