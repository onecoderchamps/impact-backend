public interface IScraperService
{
    Task<Object> scraperTiktok(TikTokProfileRequest items, string idUser);
    Task<Object> scraperInstagram(InstagramProfileRequest items, string idUser);
    Task<Object> scraperYoutube(YoutubeProfileRequest items, string idUser);
    Task<Object> scraperLinkin(LinkinProfileRequest items, string idUser);



}