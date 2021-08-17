using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;
using OlegChibikov.ZendeskInterview.Marketplace.Core;

namespace OlegChibikov.ZendeskInterview.Marketplace.Tests
{
    public class OutputFormatterTests
    {
        [Test]
        public void PrintsNoDataForNullObject()
        {
            // Arrange
            var sut = new OutputFormatter();

            // Act
            var output = sut.ListProperties(null);

            // Assert
            Assert.That(output, Is.EqualTo(OutputFormatter.NoData));
        }

        [Test]
        public void PrintsFilledProperties()
        {
            // Arrange
            var dateTimeOffset = DateTimeOffset.Now;
            var externalId = Guid.NewGuid();
            var sut = new OutputFormatter();

            // Act
            var output = sut.ListProperties(new Organization { CreatedAt = dateTimeOffset, Details = "1", DomainNames = Array.Empty<string>(), ExternalId = externalId });

            // Assert
            AssertValuePresent(output, "ExternalId", externalId.ToString());
            AssertValuePresent(output, "CreatedAt", dateTimeOffset.ToString());
        }

        [Test]
        public void PrintsNoDataForMissingProperties()
        {
            // Arrange
            var sut = new OutputFormatter();

            // Act
            var output = sut.ListProperties(new Organization());

            // Assert
            AssertValuePresent(output, "DomainNames", OutputFormatter.NoData);
        }

        [Test]
        public void PrintsRelatedEntityWithIndentation()
        {
            // Arrange
            const string details = "Blah";
            var sut = new OutputFormatter();

            // Act
            var output = sut.ListProperties(new User { Organization = new Organization { Details = details } });

            // Assert
            AssertValuePresent(output, "   Details", details);
        }

        [Test]
        public void PrintsArrayAsCommaSeparatedValues()
        {
            // Arrange
            var sut = new OutputFormatter();

            // Act
            var output = sut.ListProperties(new Organization { Tags = new[] { "1", "2" } });

            // Assert
            AssertValuePresent(output, "Tags", "1, 2");
        }

        static void AssertValuePresent(string output, string propertyName, string value)
        {
            Assert.That(Regex.IsMatch(output, @$"{propertyName}:\s*{Regex.Escape(value)}"), propertyName + " - " + value + " is missing: " + output);
        }
    }
}
