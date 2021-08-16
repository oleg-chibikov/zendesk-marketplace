using System;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class UserRelatedEntitiesLoader : IRelatedEntitiesLoader
    {
        readonly ILiteDbRepository<Organization> _organizationRepository;

        public UserRelatedEntitiesLoader(ILiteDbRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        }

        public void LoadRelatedEntities(object mainEntity)
        {
            _ = mainEntity ?? throw new ArgumentNullException(nameof(mainEntity));

            var user = (User)mainEntity;
            user.Organization = _organizationRepository.FindById(user.OrganizationId);
        }
    }
}