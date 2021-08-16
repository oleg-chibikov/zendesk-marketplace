using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LiteDB;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;
using OlegChibikov.ZendeskInterview.Marketplace.Core;
using OlegChibikov.ZendeskInterview.Marketplace.DAL;

namespace OlegChibikov.ZendeskInterview.Marketplace.Launcher
{
    static class Program
    {
        static async Task Main()
        {
            BsonMapper.Global.RegisterType<DateTimeOffset>(
                (value) =>
                {
                    return new BsonValue(value.UtcDateTime);
                },
                (bson) =>
                {
                    return bson.AsDateTime.ToUniversalTime();
                });

            var organizationFormatter = new OrganizationFormatter();
            var userFormatter = new UserFormatter(organizationFormatter);
            var ticketFormatter = new TicketFormatter(organizationFormatter, userFormatter);

            using var organizationRepository = await CreateRepositoryAsync<Organization>().ConfigureAwait(false);
            using var usersRepository = await CreateRepositoryAsync<User>().ConfigureAwait(false);
            using var ticketsRepository = await CreateRepositoryAsync<Ticket>().ConfigureAwait(false);

            var relatedEntityLoaders = new Dictionary<Type, IRelatedEntitiesLoader>
            {
                { typeof(User), new UserRelatedEntitiesLoader(organizationRepository) },
                { typeof(Ticket), new TicketRelatedEntitiesLoader(organizationRepository, usersRepository) }
            };

            var entityFormatters = new Dictionary<Type, IEntityFormatter>
            {
                { typeof(User), userFormatter },
                { typeof(Ticket), ticketFormatter },
                { typeof(Organization), organizationFormatter }
            };

            var repositories = new Dictionary<Type, IEntitySearcher>
            {
                { typeof(User), usersRepository },
                { typeof(Ticket), ticketsRepository },
                { typeof(Organization), organizationRepository }
            };

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Type? entityType;
                while ((entityType = GetEntityTypeFromUserInput()) == null)
                {
                }

                PropertyInfo? propertyInfo;
                while ((propertyInfo = GetFieldInfoFromUserInput(entityType)) == null)
                {
                }

                object? searchValue;
                while ((searchValue = GetSearchValueFromUserInput(propertyInfo)) == null)
                {
                }

                if (!repositories.TryGetValue(entityType, out var entitySearcher))
                {
                    throw new InvalidOperationException($"Entity formatter is not registered for {entityType.Name}");
                }

                var searchResults = entitySearcher.Find(propertyInfo.Name, searchValue, propertyInfo.IsCollection()).ToArray();
                if (!searchResults.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No results");
                    continue;
                }

                if (relatedEntityLoaders.TryGetValue(entityType, out var relatedEntitiesLoader))
                {
                    foreach (var searchResult in searchResults)
                    {
                        relatedEntitiesLoader.LoadRelatedEntities(searchResult);
                    }
                }

                if (!entityFormatters.TryGetValue(entityType, out var entityFormatter))
                {
                    throw new InvalidOperationException($"Entity formatter is not registered for {entityType.Name}");
                }

                var formattedResults = searchResults.Select(x=>x.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Join(Environment.NewLine, formattedResults));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Do you wish to continue? (y/n)");
                var key = Console.ReadLine();
                if (key != null && key.ToUpperInvariant() == "N")
                {
                    return;
                }
            }
        }

        static async Task<ILiteDbRepository<T>> CreateRepositoryAsync<T>()
        {
            var loader = new ResourceLoader<T>($"{typeof(T).Name.ToLowerInvariant()}s");
            var repository = new LiteDbRepository<T>(loader);
            await repository.LoadInitialDataIfNeededAsync().ConfigureAwait(false);
            return repository;
        }

        static object? GetSearchValueFromUserInput(PropertyInfo propertyInfo)
        {
            Console.Write("Enter search value (full match expected): ");
            var searchValue = Console.ReadLine();

            if (propertyInfo.PropertyType == typeof(bool))
            {
                return bool.TryParse(searchValue, out var v) ? v : null;
            }

            if (propertyInfo.PropertyType == typeof(int))
            {
                return int.TryParse(searchValue, out var v) ? v : null;
            }

            if (propertyInfo.PropertyType == typeof(Guid))
            {
                return Guid.TryParse(searchValue, out var v) ? v : null;
            }

            if (propertyInfo.PropertyType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.TryParse(searchValue, out var v) ? v : null;
            }

            return searchValue;
        }

        static PropertyInfo? GetFieldInfoFromUserInput(Type entityType)
        {
            Console.WriteLine("Please choose a field to search");
            var i = 0;
            var properties = new Dictionary<int, PropertyInfo>();
            foreach (var propertyInfo in entityType.GetProperties())
            {
                properties.Add(++i, propertyInfo);
                Console.WriteLine($"{i} - {propertyInfo.Name}");
            }

            Console.Write("Your choice: ");

            var fieldType = Console.ReadLine();
            if (fieldType == null)
            {
                return null;
            }

            if (!int.TryParse(fieldType, out var fieldIndex) || !properties.ContainsKey(fieldIndex))
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