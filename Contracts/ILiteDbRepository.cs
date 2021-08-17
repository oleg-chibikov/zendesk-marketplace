using System.Threading.Tasks;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface ILiteDbRepository<out TEntity> : IEntitySearcher
    {
        TEntity? FindById<TId>(TId value);

        Task LoadInitialDataIfNeededAsync();
    }
}