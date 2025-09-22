public interface IUserService
{
    Task<Object> Get();
   Task<Object> GetKOL(string category);
    Task<Object> TransferBalance(CreateTransferDto item, string idUser);

}