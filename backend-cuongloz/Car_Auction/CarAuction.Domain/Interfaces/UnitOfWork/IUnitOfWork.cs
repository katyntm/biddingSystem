namespace CarAuction.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task SaveChangeAsync();
    }
}
