using System;
using System.Collections.Generic;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface IEntitySearcher : IDisposable
    {
        IEnumerable<object> Find(string propertyName, object? value, bool isCollection);
    }
}