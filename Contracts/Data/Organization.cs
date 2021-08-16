using System.Collections.Generic;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data
{
    public class Organization : Entity<int>
    {
        public string Name { get; set; }

        public IReadOnlyCollection<string> DomainNames { get; set; }

        public string Details { get; set; }

        public bool SharedTickets { get; set; }
    }
}