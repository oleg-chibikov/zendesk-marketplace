using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class UserInputReceiver : IUserInputReceiver
    {
        public object? WaitForSearchValue(PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            object? searchValue = null;
            var needRetry = true;
            while (needRetry)
            {
                searchValue = GetSearchValueFromUserInput(propertyInfo, out needRetry);
            }

            return searchValue;
        }

        public PropertyInfo WaitForPropertyInfo(Type entityType)
        {
            _ = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PropertyInfo? propertyInfo;
            while ((propertyInfo = GetPropertyInfoFromUserInput(entityType)) == null)
            {
            }

            return propertyInfo;
        }

        public Type WaitForEntityType()
        {
            Type? entityType;
            while ((entityType = GetEntityTypeFromUserInput()) == null)
            {
            }

            return entityType;
        }

        static object? GetSearchValueFromUserInput(PropertyInfo propertyInfo, out bool needRetry)
        {
            Console.Write("Enter search value (full match expected): ");
            var searchValue = Console.ReadLine();
            needRetry = true;

            if (searchValue == null)
            {
                needRetry = false;
                return null;
            }

            searchValue = searchValue.Trim();

            if (propertyInfo.PropertyType == typeof(bool))
            {
                if (!bool.TryParse(searchValue, out var v))
                {
                    return null;
                }

                needRetry = false;
                return v;
            }

            if (propertyInfo.PropertyType == typeof(int))
            {
                if (!int.TryParse(searchValue, out var v))
                {
                    return null;
                }

                needRetry = false;
                return v;
            }

            if (propertyInfo.PropertyType == typeof(Guid))
            {
                if (!Guid.TryParse(searchValue, out var v))
                {
                    return null;
                }

                needRetry = false;
                return v;
            }

            if (propertyInfo.PropertyType == typeof(DateTimeOffset))
            {
                if (!DateTimeOffset.TryParse(searchValue, out var v))
                {
                    return null;
                }

                needRetry = false;
                return v;
            }

            needRetry = false;
            return string.IsNullOrWhiteSpace(searchValue) ? null : searchValue;
        }

        static PropertyInfo? GetPropertyInfoFromUserInput(Type entityType)
        {
            Console.WriteLine("Please choose a field to search");
            var i = 0;
            var properties = new Dictionary<int, PropertyInfo>();
            foreach (var propertyInfo in entityType.GetProperties().OrderBy(x => x.Name))
            {
                var isCustomType = propertyInfo.IsCustomType();
                if (isCustomType)
                {
                    continue;
                }

                properties.Add(++i, propertyInfo);
                Console.WriteLine($"{i} - {propertyInfo.Name}");
            }

            Console.Write("Your choice: ");

            var fieldType = Console.ReadLine();
            if (fieldType == null)
            {
                return null;
            }

            if (!int.TryParse(fieldType.Trim(), out var fieldIndex) || !properties.ContainsKey(fieldIndex))
            {
                return null;
            }

            return properties[fieldIndex];
        }

        static Type? GetEntityTypeFromUserInput()
        {
            Console.WriteLine("Please select the table to search");
            Console.WriteLine("1 - Organizations");
            Console.WriteLine("2 - Users");
            Console.WriteLine("3 - Tickets");
            Console.Write("Your choice: ");
            var type = Console.ReadLine();
            if (type == null)
            {
                return null;
            }

            return type.Trim() switch
            {
                "1" => typeof(Organization),
                "2" => typeof(User),
                "3" => typeof(Ticket),
                _ => null
            };
        }
    }
}