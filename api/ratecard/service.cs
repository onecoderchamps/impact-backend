using impact.Shared.Models;
using MongoDB.Driver;

namespace RepositoryPattern.Services.RateCardService
{
    public class RateCardService : IRateCardService
    {
        private readonly IMongoCollection<RateCard> _rateCardCollection;

        public RateCardService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("impact");
            _rateCardCollection = database.GetCollection<RateCard>("RateCard");
        }

        public async Task<object> GetByUser(string idUser)
        {
            try
            {
                var rateCard = await _rateCardCollection.Find(rc => rc.IdUser == idUser).FirstOrDefaultAsync();
                return new { code = 200, data = rateCard, message = "Rate card retrieved" };
            }
            catch (Exception)
            {
                throw new CustomException(500, "Error", "Failed to retrieve rate card.");
            }
        }

        public async Task<object> Post(CreateOrUpdateRateCardDto dto, string idUser)
        {
            try
            {
                var existing = await _rateCardCollection.Find(x => x.IdUser == idUser).FirstOrDefaultAsync();
                if (existing != null)
                {
                    throw new CustomException(400, "Error", "Rate card already exists for this user.");
                }

                var rateCard = new RateCard
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = idUser,
                    Currency = dto.Currency,
                    Rates = dto.Rates,
                    UpdatedAt = DateTime.UtcNow
                };

                await _rateCardCollection.InsertOneAsync(rateCard);

                return new { code = 200, id = rateCard.Id, message = "Rate card created." };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateOrUpdateRateCardDto dto)
        {
            try
            {
                var existing = await _rateCardCollection.Find(x => x.IdUser == id).FirstOrDefaultAsync();
                if (existing == null)
                {
                    var rateCard = new RateCard
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdUser = id,
                        Currency = dto.Currency,
                        Rates = dto.Rates,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _rateCardCollection.InsertOneAsync(rateCard);
                }

                existing.Currency = dto.Currency;
                existing.Rates = dto.Rates;
                existing.UpdatedAt = DateTime.UtcNow;

                await _rateCardCollection.ReplaceOneAsync(x => x.IdUser == id, existing);

                return new { code = 200, id = existing.Id, message = "Rate card updated." };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}
