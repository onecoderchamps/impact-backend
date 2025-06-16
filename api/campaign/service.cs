using MongoDB.Driver;
using impact.Shared.Models;

namespace RepositoryPattern.Services.CampaignService
{
    public class CampaignService : ICampaignService
    {
        private readonly IMongoCollection<Campaign> dataUser;
        private readonly string key;

        public CampaignService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("impact");
            dataUser = database.GetCollection<Campaign>("Campaign");
        }
        public async Task<Object> Get()
        {
            try
            {
                var items = await dataUser.Find(_ => true).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<Object> GetById(string id)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Id == id).FirstOrDefaultAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Post(CreateCampaignDto item, string idUser)
        {
            try
            {
                var CampaignData = new Campaign()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = idUser,
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                await dataUser.InsertOneAsync(CampaignData);
                return new { code = 200, id = CampaignData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateCampaignDto item)
        {
            try
            {
                var CampaignData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                await dataUser.ReplaceOneAsync(x => x.Id == id, CampaignData);
                return new { code = 200, id = CampaignData.Id.ToString(), message = "Data Updated" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Delete(string id)
        {
            try
            {
                var CampaignData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                CampaignData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, CampaignData);
                return new { code = 200, id = CampaignData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}