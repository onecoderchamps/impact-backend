public interface IUserService
{
    Task<Object> Get();
    Task<Object> TransferBalance(CreateTransferDto item, string idUser);

}