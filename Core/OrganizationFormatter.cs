using System;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class OrganizationFormatter : IEntityFormatter
    {
        public string Format(object entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var organization = (Organization)entity;
            return organization.Name;
        }
    }
}
