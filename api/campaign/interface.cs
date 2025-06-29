public interface ICampaignService
{
    Task<Object> Get(string id);
    Task<Object> GetAll();

    Task<Object> GetById(string id);
    Task<Object> Post(CreateCampaignDto items,string idUser);
    Task<Object> RegisterCampaign(RegisterCampaignDto items, string idUser);
    Task<Object> RegisterByBrandCampaign(RegisterCampaignDto items, string idUser);
    
    Task<Object> RegisterMemberCampaign(string items);
    Task<Object> MemberCampaign(UpdateCampaignDto items);
    Task<Object> Put(string id, CreateCampaignDto items);
    Task<Object> Delete(string id);
}