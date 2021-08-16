using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public static class PropertyInfoExtensions
    {
        public static bool IsCollection(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            return IsCollection(propertyInfo.PropertyType);
        }

        public static bool IsCustomType(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            return propertyInfo.PropertyType.Assembly == typeof(Organization).Assembly;
        }

        public static string PropertyList(this object? obj, int prefixSpaces = 0)
        {
            if (obj == null)
            {
                return "No data";
            }

            var props = obj.GetType().GetProperties();
            var sb = new StringBuilder();
            const int leftColumnWidth = 30;
            const int indentation = 3;
            foreach (var p in props.OrderBy(IsCustomType))
            {
                sb.AppendLine();
                var spacesNeeded = leftColumnWidth - p.Name.Length - 1;
                if (prefixSpaces > 0)
                {
                    sb.Append(new string(' ', prefixSpaces));
                }

                sb.Append(p.Name).Append(":");
                var value = p.GetValue(obj, null);
                var isCustomType = p.IsCustomType();

                if (isCustomType)
                {
                    sb.AppendLine();
                }
                else
                {
                    if (spacesNeeded > 0)
                    {
                        sb.Append(new string(' ', spacesNeeded));
                    }
                }

                if (value != null && IsCollection(p.PropertyType))
                {
                    var enumerable = (IEnumerable)value;
                    value = string.Join(", ", enumerable.Cast<string>());
                }

                if (isCustomType)
                {
                    sb.Append(value.PropertyList(prefixSpaces + indentation));
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        static bool IsCollection(this Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}