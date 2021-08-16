using System;
using System.Threading.Tasks;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface ILiteDbRepository<out TEntity> : IDisposable, IEntitySearcher
    {
        TEntity? FindById<TId>(TId value);

        Task LoadInitialDataIfNeededAsync();
    }
}