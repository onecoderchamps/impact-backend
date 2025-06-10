public interface IBannerService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<Object> Post(CreateBannersDto items);
    Task<Object> Put(string id, CreateBannersDto items);
    Task<Object> Delete(string id);
}