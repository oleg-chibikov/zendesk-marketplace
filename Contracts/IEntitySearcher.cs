using System.Collections.Generic;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface IEntitySearcher
    {
        IEnumerable<object> Find(string propertyName, object value, bool isCollection);
    }
}