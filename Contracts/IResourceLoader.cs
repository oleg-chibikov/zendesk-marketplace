using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface IResourceLoader<T>
    {
        Task<IEnumerable<T>> LoadEntitiesAsync();
    }
}