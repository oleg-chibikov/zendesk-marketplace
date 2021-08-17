using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            LiteDbUtilities.RegisterTypes();
            var outputFormatter = new OutputFormatter();
            var userInputReceiver = new UserInputReceiver();
            var repositories = await CreateRepositoriesAsync().ConfigureAwait(false);
            var searchRequestProcessor = new SearchRequestProcessor(userInputReceiver, repositories, outputFormatter);
            try
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    var output = searchRequestProcessor.ProcessSearchRequest();
                    if (output == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No results");
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(output);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("Do you wish to continue? (y/n)");
                    var key = Console.ReadLine();
                    if (key != null && key.ToUpperInvariant() == "N")
                    {
                        return;
                    }
                }
            }
            finally
            {
                foreach (var repository in repositories.Values)
                {
                    repository.Dispose();
                }
            }
        }

        static async Task<Dictionary<Type, IEntitySearcher>> CreateRepositoriesAsync()
        {
            static async Task<ILiteDbRepository<T>> CreateRepositoryAsync<T>(IRelatedEntitiesLoader? relatedEntitiesLoader = null)
            {
                var loader = new ResourceLoader<T>($"{typeof(T).Name.ToLowerInvariant()}s");
                var repository = new LiteDbRepository<T>(loader, relatedEntitiesLoader);
                await repository.LoadInitialDataIfNeededAsync().ConfigureAwait(false);
                return repository;
            }

            var organizationRepository = await CreateRepositoryAsync<Organization>().ConfigureAwait(false);
            var userRelatedEntitiesLoader = new UserRelatedEntitiesLoader(organizationRepository);
            var usersRepository = await CreateRepositoryAsync<User>(userRelatedEntitiesLoader).ConfigureAwait(false);
            var ticketRelatedEntitiesLoader = new TicketRelatedEntitiesLoader(organizationRepository, usersRepository, userRelatedEntitiesLoader);
            var ticketsRepository = await CreateRepositoryAsync<Ticket>(ticketRelatedEntitiesLoader).ConfigureAwait(false);

            return new Dictionary<Type, IEntitySearcher>
            {
                { typeof(User), usersRepository },
                { typeof(Ticket), ticketsRepository },
                { typeof(Organization), organizationRepository }
            };
        }
    }
}