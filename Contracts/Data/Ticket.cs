using System;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data
{
    public class Ticket : Entity<Guid>
    {
        public string? Type { get; set; }

        public string Subject { get; set; }

        public string? Description { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public int SubmitterId { get; set; }

        public int AssigneeId { get; set; }

        public int OrganizationId { get; set; }

        public bool HasIncidents { get; set; }

        public DateTimeOffset DueAt { get; set; }

        public string Via { get; set; }

        public Organization? Organization { get; set; }

        public User? Submitter { get; set; }

        public User? Assignee { get; set; }
    }
}