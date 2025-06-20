public interface IUserService
{
    Task<Object> Get();
   Task<Object> GetKOL();
    Task<Object> TransferBalance(CreateTransferDto item, string idUser);

}