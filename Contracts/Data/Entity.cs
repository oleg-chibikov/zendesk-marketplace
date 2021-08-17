using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data
{
    public abstract class Entity<TId>
    {
        [JsonProperty("_id")]
        public TId Id { get; set; }

        public Uri Url { get; set; }

        public Guid ExternalId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }
}