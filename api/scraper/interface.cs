public interface IScraperService
{
    Task<Object> GetById(string id);
    Task<Object> scraperTiktok(string idUser);
    Task<Object> scraperInstagram(InstagramProfileRequest items, string idUser);
    Task<Object> scraperYoutube(YoutubeProfileRequest items, string idUser);
    Task<Object> scraperLinkin(LinkinProfileRequest items, string idUser);



}