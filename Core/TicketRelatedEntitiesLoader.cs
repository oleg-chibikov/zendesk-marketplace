using System;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class TicketRelatedEntitiesLoader : IRelatedEntitiesLoader
    {
        readonly ILiteDbRepository<Organization> _organizationsRepository;
        readonly ILiteDbRepository<User> _usersRepository;
        readonly IRelatedEntitiesLoader _useRelatedEntitiesLoader;

        public TicketRelatedEntitiesLoader(ILiteDbRepository<Organization> organizationsRepository, ILiteDbRepository<User> usersRepository, IRelatedEntitiesLoader useRelatedEntitiesLoader)
        {
            _organizationsRepository = organizationsRepository ?? throw new ArgumentNullException(nameof(organizationsRepository));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _useRelatedEntitiesLoader = useRelatedEntitiesLoader ?? throw new ArgumentNullException(nameof(useRelatedEntitiesLoader));
        }

        public void LoadRelatedEntities(object mainEntity)
        {
            _ = mainEntity ?? throw new ArgumentNullException(nameof(mainEntity));

            var ticket = (Ticket)mainEntity;
            ticket.Organization = _organizationsRepository.FindById(ticket.OrganizationId);
            ticket.Assignee = _usersRepository.FindById(ticket.AssigneeId);
            ticket.Submitter = _usersRepository.FindById(ticket.SubmitterId);
            if (ticket.Assignee != null)
            {
                _useRelatedEntitiesLoader.LoadRelatedEntities(ticket.Assignee);
            }

            if (ticket.Submitter != null)
            {
                _useRelatedEntitiesLoader.LoadRelatedEntities(ticket.Submitter);
            }
        }
    }
}