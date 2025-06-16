public interface ICampaignService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<Object> Post(CreateCampaignDto items,string idUser);
    Task<Object> Put(string id, CreateCampaignDto items);
    Task<Object> Delete(string id);
}