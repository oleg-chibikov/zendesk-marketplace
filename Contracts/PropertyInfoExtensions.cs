using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public static class PropertyInfoExtensions
    {
        public static bool IsCollection(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            return propertyInfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);
        }

        public static string? PropertyList(this object? obj)
        {
            if (obj == null)
            {
                return null;
            }

            var props = obj.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                sb.AppendLine(p.Name + ": " + p.GetValue(obj, null));
            }

            return sb.ToString();
        }
    }
}