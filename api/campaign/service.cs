using MongoDB.Driver;
using impact.Shared.Models;

namespace RepositoryPattern.Services.CampaignService
{
    public class CampaignService : ICampaignService
    {
        private readonly IMongoCollection<Campaign> dataUser;
        private readonly IMongoCollection<MemberCampaign> dataListCampaignUser;
        private readonly IMongoCollection<User> user;


        private readonly string key;

        public CampaignService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("impact");
            dataUser = database.GetCollection<Campaign>("Campaign");
            dataListCampaignUser = database.GetCollection<MemberCampaign>("MemberCampaign");
            user = database.GetCollection<User>("User");


        }
        public async Task<Object> Get(string id)
        {
            try
            {
                var items = await dataUser.Find(_ => _.IdUser == id).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var items = await dataUser.Find(_ => _.IsActive == true).ToListAsync();
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
                    StartDate = DateTime.Parse(item.StartDate),
                    EndDate = DateTime.Parse(item.EndDate),
                    JenisPekerjaan = item.JenisPekerjaan,
                    HargaPekerjaan = item.HargaPekerjaan,
                    TipeKonten = item.TipeKonten,
                    ReferensiVisual = item.ReferensiVisual,
                    ArahanKonten = item.ArahanKonten,
                    ArahanCaption = item.ArahanCaption,
                    Catatan = item.Catatan,
                    Website = item.Website,
                    Hashtag = item.Hashtag,
                    MentionAccount = item.MentionAccount,
                    NamaProyek = item.NamaProyek,
                    TipeProyek = item.TipeProyek,
                    CoverProyek = item.CoverProyek,
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

        public async Task<object> RegisterCampaign(RegisterCampaignDto item, string idUser)
        {
            try
            {
                var CampaignData = await dataUser.Find(x => x.Id == item.IdCampaign).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                var checkUserCampaign = await dataListCampaignUser.Find(x => x.IdUser == idUser && x.IdCampaign == item.IdCampaign).FirstOrDefaultAsync();
                if (checkUserCampaign != null)
                {
                    throw new CustomException(400, "Error", "You have already registered for this campaign");
                }
                var MemberCampaignData = new MemberCampaign()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = idUser,
                    IdCampaign = item.IdCampaign,
                    InviteBy = "User",
                    Status = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                await dataListCampaignUser.InsertOneAsync(MemberCampaignData);
                return new { code = 200, id = MemberCampaignData.Id, message = "Register Success" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> RegisterByBrandCampaign(RegisterCampaignDto item, string idUser)
        {
            try
            {
                var CampaignData = await dataUser.Find(x => x.Id == item.IdCampaign).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                var checkUserCampaign = await dataListCampaignUser.Find(x => x.IdUser == idUser && x.IdCampaign == item.IdCampaign).FirstOrDefaultAsync();
                if (checkUserCampaign != null)
                {
                    throw new CustomException(400, "Error", "You have already registered for this campaign");
                }
                var MemberCampaignData = new MemberCampaign()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = idUser,
                    IdCampaign = item.IdCampaign,
                    InviteBy = "Brand",
                    Status = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                await dataListCampaignUser.InsertOneAsync(MemberCampaignData);
                return new { code = 200, id = MemberCampaignData.Id, message = "Register Success" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> RegisterMemberCampaign(string item)
        {
            try
            {
                var CampaignData = await dataUser.Find(x => x.Id == item).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                var checkUserCampaign = await dataListCampaignUser.Find(x => x.IdCampaign == item).ToListAsync();
                var kolUsersWithScraperData = new List<object>();
                foreach (var items in checkUserCampaign)
                {
                    var users = await user.Find(x => x.Id == items.IdUser).FirstOrDefaultAsync();

                    kolUsersWithScraperData.Add(new
                    {
                        Id = items.Id,
                        IdUser = items.IdUser,
                        IdCampaign = items.IdCampaign,
                        Status = items.Status,
                        fullName = users.FullName,
                        image = users.Image,
                        Email = users.Email,
                        InviteBy = items.InviteBy,
                        IsActive = items.IsActive,
                        CreatedAt = items.CreatedAt
                    });
                }
                return new { code = 200, Data = kolUsersWithScraperData, message = "Success" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> MemberCampaign(UpdateCampaignDto item)
        {
            try
            {
                var CampaignData = await dataListCampaignUser.Find(x => x.IdCampaign == item.IdCampaign && x.IdUser == item.IdUser).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                CampaignData.Status = item.Status switch
                {
                    "Approved" => true,
                    "Rejected" => false,
                    _ => CampaignData.Status
                };
                await dataListCampaignUser.ReplaceOneAsync(x => x.IdCampaign == item.IdCampaign && x.IdUser == item.IdUser, CampaignData);
                return new { code = 200, id = CampaignData.Id.ToString(), message = "Data Updated" };
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