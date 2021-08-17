using System;
using System.Collections;
using System.Reflection;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public static class PropertyInfoExtensions
    {
        public static bool IsCollection(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            return propertyInfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);
        }

        public static bool IsCustomType(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            return propertyInfo.PropertyType.Assembly == typeof(Organization).Assembly;
        }
    }
}