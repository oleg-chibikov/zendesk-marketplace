using System;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data
{
    public class User : Entity<int>
    {
        public string Name { get; set; }

        public string? Alias { get; set; }

        public bool Active { get; set; }

        public bool Verified { get; set; }

        public bool Shared { get; set; }

        public string? Locale { get; set; }

        public string? Timezone { get; set; }

        public DateTimeOffset LastLoginAt { get; set; }

        public string? Email { get; set; }

        public string Phone { get; set; }

        public string Signature { get; set; }

        public int OrganizationId { get; set; }

        public bool Suspended { get; set; }

        public string Role { get; set; }

        public Organization? Organization { get; set; }
    }
}