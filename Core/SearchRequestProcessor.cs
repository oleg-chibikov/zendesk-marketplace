using System;
using System.Collections.Generic;
using System.Linq;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class SearchRequestProcessor : ISearchRequestProcessor
    {
        internal static readonly string Separator = Environment.NewLine + "---------------------" + Environment.NewLine;
        readonly IUserInputReceiver _userInputReceiver; readonly IReadOnlyDictionary<Type, IEntitySearcher> _repositories; readonly IOutputFormatter _outputFormatter;

        public SearchRequestProcessor(IUserInputReceiver userInputReceiver, IReadOnlyDictionary<Type, IEntitySearcher> repositories, IOutputFormatter outputFormatter)
        {
            _userInputReceiver = userInputReceiver ?? throw new ArgumentNullException(nameof(userInputReceiver));
            _repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));
            _outputFormatter = outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter));
        }

        public string? ProcessSearchRequest()
        {
            var entityType = _userInputReceiver.WaitForEntityType();
            var propertyInfo = _userInputReceiver.WaitForPropertyInfo(entityType);
            var searchValue = _userInputReceiver.WaitForSearchValue(propertyInfo);

            if (!_repositories.TryGetValue(entityType, out var entitySearcher))
            {
                throw new InvalidOperationException($"Repository is not registered for {entityType.Name}");
            }

            var searchResults = entitySearcher.Find(propertyInfo.Name, searchValue, propertyInfo.IsCollection()).ToArray();
            if (!searchResults.Any())
            {
                return null;
            }

            var formattedResults = searchResults.Select(_outputFormatter.ListProperties);
            return string.Join(Separator, formattedResults);
        }
    }
}