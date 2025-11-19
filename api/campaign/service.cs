using MongoDB.Driver;
using impact.Shared.Models;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Globalization;

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
                var items = await dataUser.Find(_ => _.IsActive == true && _.IsVerification == true).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> GetKontrak(string id)
        {
            try
            {
                // Ambil semua kontrak yang sesuai
                var kontrakList = await dataListCampaignUser.Find(_ => _.Status == true && _.IdUser == id).ToListAsync();

                // Siapkan hasil akhir
                var result = new List<object>();

                foreach (var kontrak in kontrakList)
                {
                    // Ambil user detail berdasarkan IdUser dari kontrak
                    var userDetail = await dataUser.Find(u => u.Id == kontrak.IdCampaign).FirstOrDefaultAsync();

                    // Gabungkan kontrak + userDetail ke dalam satu objek
                    result.Add(new
                    {
                        kontrak = kontrak,
                        detail = userDetail,
                        brand = await user.Find(u => u.Id == userDetail.IdUser).FirstOrDefaultAsync(),
                        influencer = await user.Find(u => u.Id == kontrak.IdUser).FirstOrDefaultAsync()
                    });
                }

                return new
                {
                    code = 200,
                    message = "Data retrieved successfully",
                    data = result
                };
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

        public async Task<object> PostActivate(PayCampaignDto item)
        {
            try
            {
                string merchantCode = "DS25522";
                string apiKey = "2583c8f79130c1534e123479183585cc";

                // int paymentAmount = item.HargaPekerjaan;
                string merchantOrderId = item.IdCampaign;

                // Generate signature string
                string rawSignature = merchantCode + merchantOrderId + item.HargaPekerjaan + apiKey;

                // Compute SHA256 hash signature
                string signature;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(rawSignature);
                    byte[] hash = md5.ComputeHash(bytes);
                    signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }

                var requestBody = new
                {
                    merchantCode = merchantCode,
                    paymentAmount = item.HargaPekerjaan,
                    paymentMethod = "BC",
                    merchantOrderId = merchantOrderId,
                    productDetails = "Pembayaran untuk Toko Contoh",
                    additionalParam = "",
                    merchantUserInfo = "",
                    customerVaName = "John Doe",
                    email = "test@test.com",
                    phoneNumber = "08123456789",
                    itemDetails = new List<ItemDetail>
                    {
                        new ItemDetail { name = "Aktifasi Iklan", price = item.HargaPekerjaan ?? 0, quantity = 1 }
                    },
                    callbackUrl = "https://apiimpact.coderchamps.co.id/api/v1/campaign/verifCampaign",
                    returnUrl = "https://impact.id/appimpact/daftarCampaign",
                    signature = signature,
                    expiryPeriod = 10
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);

                using var httpClient = new HttpClient();
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("https://sandbox.duitku.com/webapi/api/merchant/v2/inquiry", content);
                string responseContent = await response.Content.ReadAsStringAsync();

                return new
                {
                    code = (int)response.StatusCode,
                    request = requestBody,
                    response = JsonSerializer.Deserialize<object>(responseContent)
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 500,
                    error = ex.Message
                };
            }
        }

        public async Task<object> PayCallback(PayCallbackCampaignDto item)
        {
            try
            {
                string merchantCode = "DS25522";
                string apiKey = "2583c8f79130c1534e123479183585cc";
                string merchantOrderId = item.merchantOrderId;
                var campaign = await dataUser.Find(_ => _.Id == merchantOrderId).FirstOrDefaultAsync();
                if (campaign == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                campaign.IsVerification = true;
                await dataUser.ReplaceOneAsync(x => x.Id == merchantOrderId, campaign);
                return new
                {
                    code = 200,
                    request = "Done",
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 500,
                    error = ex.Message
                };
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

                    if (users != null)
                    {
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
                    else
                    {
                        // Jika user tidak ditemukan, bisa juga tambahkan info kosong atau log:
                        kolUsersWithScraperData.Add(new
                        {
                            Id = items.Id,
                            IdUser = items.IdUser,
                            IdCampaign = items.IdCampaign,
                            Status = items.Status,
                            fullName = "(User Not Found)",
                            image = "",
                            Email = "",
                            InviteBy = items.InviteBy,
                            IsActive = items.IsActive,
                            CreatedAt = items.CreatedAt
                        });
                    }
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

        public async Task<object> ApproveContract(string id)
        {
            try
            {
                var CampaignData = await dataListCampaignUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                CampaignData.IsVerification = true;
                await dataListCampaignUser.ReplaceOneAsync(x => x.Id == id, CampaignData);
                return new { code = 200, id = CampaignData.Id.ToString(), message = "Data Updated" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> DeclineContract(string id)
        {
            try
            {
                var CampaignData = await dataListCampaignUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (CampaignData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                CampaignData.IsVerification = false;
                await dataListCampaignUser.ReplaceOneAsync(x => x.Id == id, CampaignData);
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