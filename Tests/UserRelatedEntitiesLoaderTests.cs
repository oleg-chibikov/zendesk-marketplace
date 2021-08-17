using Moq;
using NUnit.Framework;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;
using OlegChibikov.ZendeskInterview.Marketplace.DAL;

namespace OlegChibikov.ZendeskInterview.Marketplace.Tests
{
    public class UserRelatedEntitiesLoaderTests
    {
        [Test]
        public void SetsOrganization()
        {
            // Arrange
            const int organizationId = 99;
            var user = new User { OrganizationId = organizationId };
            var organization = new Organization();
            var organizationRepositoryMock = new Mock<ILiteDbRepository<Organization>>();
            organizationRepositoryMock.Setup(x => x.FindById(organizationId)).Returns(organization);
            var sut = new UserRelatedEntitiesLoader(organizationRepositoryMock.Object);

            // Act
            sut.LoadRelatedEntities(user);

            // Assert
            Assert.That(user.Organization, Is.EqualTo(organization));
        }
    }
}
