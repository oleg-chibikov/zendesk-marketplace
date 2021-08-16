using System;
using System.Text;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class TicketFormatter : IEntityFormatter
    {
        readonly IEntityFormatter _organizationFormatter;
        readonly IEntityFormatter userFormatter;

        public TicketFormatter(IEntityFormatter organizationFormatter, IEntityFormatter userFormatter)
        {
            _organizationFormatter = organizationFormatter ?? throw new ArgumentNullException(nameof(organizationFormatter));
            this.userFormatter = userFormatter ?? throw new ArgumentNullException(nameof(userFormatter));
        }

        public string Format(object entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var ticket = (Ticket)entity;

            var output = new StringBuilder();
            output.AppendLine(ticket.Description);
            output.AppendLine(ticket.Organization != null ? _organizationFormatter.Format(ticket.Organization) : ticket.OrganizationId.ToString());
            output.AppendLine(ticket.Assignee != null ? userFormatter.Format(ticket.Assignee) : ticket.AssigneeId.ToString());
            output.AppendLine(ticket.Submitter != null ? userFormatter.Format(ticket.Submitter) : ticket.SubmitterId.ToString());

            return output.ToString();
        }
    }
}