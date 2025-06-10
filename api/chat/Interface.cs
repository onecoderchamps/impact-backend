public interface IChatService
{

    Task<object> SendChatWAAsync(string idUser, CreateChatDto dto);
    Task<object> GetChatWAAsync(GetChatDto dto);

}