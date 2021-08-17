using System;
using System.Reflection;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface IUserInputReceiver
    {
        object? WaitForSearchValue(PropertyInfo propertyInfo);

        PropertyInfo WaitForPropertyInfo(Type entityType);

        Type WaitForEntityType();
    }
}