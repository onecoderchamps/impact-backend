using MongoDB.Driver;
using impact.Shared.Models;

namespace RepositoryPattern.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> dataUser;
        private readonly IMongoCollection<Transaksi> dataTransaksi;
        private readonly string key;

        private readonly IMongoCollection<Scraper> _scraperCollection;
        private readonly IMongoCollection<RateCard> _rateCardCollection;


        public UserService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("impact");
            dataUser = database.GetCollection<User>("User");
            dataTransaksi = database.GetCollection<Transaksi>("Transaksi");
            _scraperCollection = database.GetCollection<Scraper>("Scraper");
            _rateCardCollection = database.GetCollection<RateCard>("RateCard");

            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get()
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

        public async Task<object> GetKOL(string category)
        {
            try
            {
                // 1. Ambil semua user
                var allUsers = await dataUser.Find(_ => _.IsActive == true && _.IdRole == "KOL").ToListAsync();

                var resultList = new List<object>();

                foreach (var user in allUsers)
                {
                    var items = await _scraperCollection.Find(_ => _.IdUser == user.Id).ToListAsync();

                    // --- 1. Category Relevancy Score (CRS) ---
                    double CRS = 10;
                    if (!string.IsNullOrEmpty(user?.Categories) && !string.IsNullOrEmpty(category))
                    {
                        var userCategories = user.Categories
                                                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                                .Select(c => c.ToLower())
                                                .ToList();

                        var inputCategories = category
                                            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                            .Select(c => c.ToLower())
                                            .ToList();

                        if (userCategories.Any(uc => inputCategories.Contains(uc)))
                            CRS = 100;
                        else if (userCategories.Any(uc => inputCategories.Any(ic => uc.Contains(ic) || ic.Contains(uc))))
                            CRS = 75;
                        else if (userCategories.Any(uc => inputCategories.Any(ic => uc.Contains(ic.Substring(0, Math.Min(3, ic.Length)))))) 
                            CRS = 40; // loosely
                        else
                            CRS = 10;
                    }

                    // --- 2. Engagement Rate (ER) ---
                    double ER = 0;
                    var tiktokData = items.FirstOrDefault(x => x.Type == "TikTok");
                    if (tiktokData?.Video != null && tiktokData.Video.Count > 0)
                    {
                        double totalEr = 0;
                        int videoCount = tiktokData.Video.Count;

                        foreach (var v in tiktokData.Video)
                        {
                            if (v.ViewCount > 0)
                            {
                                double er = (double)(v.LikeCount + v.CommentCount + v.ShareCount) / v.ViewCount;
                                totalEr += er;
                            }
                        }

                        double avgEr = totalEr / videoCount * 100; // %
                        if (avgEr > 4) ER = 95;
                        else if (avgEr >= 2) ER = 80;
                        else if (avgEr >= 1) ER = 60;
                        else ER = 30;
                    }

                    // --- Dummy Scores (sementara pakai default) ---
                    double CVP = 50; 
                    double ESP = 50;
                    double AAS = 90;
                    double CS = 70;
                    double BFS = 80;

                    // --- 3. Hitung Impact Score ---
                    double ImpactScore = (
                        CRS * 0.15 +
                        ER * 0.20 +
                        CVP * 0.15 +
                        ESP * 0.20 +
                        AAS * 0.10 +
                        CS * 0.10 +
                        BFS * 0.10
                    );

                    resultList.Add(new
                    {
                        user,
                        impactScore = Math.Round(ImpactScore, 2),
                        breakdown = new
                        {
                            CRS,
                            ER,
                            CVP,
                            ESP,
                            AAS,
                            CS,
                            BFS
                        }
                    });
                }

                return new
                {
                    code = 200,
                    data = resultList,
                    message = "Impact Score Calculated for All Users"
                };
            }
            catch (CustomException)
            {
                throw;
            }
        }


        public async Task<Object> TransferBalance(CreateTransferDto item, string idUser)
        {
            try
            {
                var from = await dataUser.Find(_ => _.Id == idUser).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data User Not Found");
                var destination = await dataUser.Find(_ => _.Phone == item.Phone).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data User Not Found");
                if (from.Balance == null)
                {
                    throw new CustomException(400, "Error", "Saldo anda tidak cukup");
                }
                if (from.Balance < item.Balance)
                {
                    throw new CustomException(400, "Error", "Saldo anda tidak cukup");
                }
                if (from.Phone == destination.Phone)
                {
                    throw new CustomException(400, "Error", "Tidak boleh kirim ke nomor yang sama");
                }
                if (item.Balance < 10000)
                {
                    throw new CustomException(400, "Error", "Minimal Transfer adalah Rp 10.000");
                }
                ///update from balance
                from.Balance -= item.Balance;
                await dataUser.ReplaceOneAsync(x => x.Id == idUser, from);

                var transaksi = new Transaksi
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = idUser,
                    IdTransaksi = Guid.NewGuid().ToString(),
                    Type = "Transfer",
                    Nominal = item.Balance,
                    Ket = "Transfer Kepada " + item.Phone,
                    Status = "Expense",
                    CreatedAt = DateTime.Now
                };
                await dataTransaksi.InsertOneAsync(transaksi);

                ///update destination balance
                destination.Balance += item.Balance;
                await dataUser.ReplaceOneAsync(x => x.Phone == item.Phone, destination);
                var transaksi2 = new Transaksi
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = item.Phone,
                    IdTransaksi = Guid.NewGuid().ToString(),
                    Type = "Transfer",
                    Nominal = item.Balance,
                    Ket = "Transfer Dari " + idUser,
                    Status = "Income",
                    CreatedAt = DateTime.Now
                };
                await dataTransaksi.InsertOneAsync(transaksi2);

                return new { code = 200, message = "Transfer Berhasil" };
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
        public async Task<object> Post(CreateUserDto item)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.FullName, item.Name);
                var user = await dataUser.Find(filter).SingleOrDefaultAsync();
                if (user != null)
                {
                    throw new CustomException(400, "Error", "Nama sudah digunakan.");
                }
                var UserData = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                await dataUser.InsertOneAsync(UserData);
                return new { code = 200, id = UserData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateUserDto item)
        {
            try
            {
                var UserData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (UserData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                UserData.FullName = item.Name;
                await dataUser.ReplaceOneAsync(x => x.Id == id, UserData);
                return new { code = 200, id = UserData.Id.ToString(), message = "Data Updated" };
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
                var UserData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (UserData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                UserData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, UserData);
                return new { code = 200, id = UserData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}