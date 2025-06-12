public interface IRateCardService
{
    Task<object> GetByUser(string idUser);
        Task<object> Post(CreateOrUpdateRateCardDto dto, string idUser);
        Task<object> Put(string id, CreateOrUpdateRateCardDto dto);
}