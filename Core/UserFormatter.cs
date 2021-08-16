using System;
using System.Text;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class UserFormatter : IEntityFormatter
    {
        readonly IEntityFormatter _organizationFormatter;

        public UserFormatter(IEntityFormatter organizationFormatter)
        {
            _organizationFormatter = organizationFormatter ?? throw new ArgumentNullException(nameof(organizationFormatter));
        }

        public string Format(object entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var user = (User)entity;
            var output = new StringBuilder();
            output.AppendLine(user.Name);
            output.AppendLine(user.Organization != null ? _organizationFormatter.Format(user.Organization) : user.OrganizationId.ToString());

            return output.ToString();
        }
    }
}