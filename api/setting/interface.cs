public interface ISettingService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<Object> Post(CreateSettingsDto items);
    Task<Object> Put(string id, CreateSettingsDto items);
    Task<Object> Delete(string id);
}