using Moq;
using NUnit.Framework;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;
using OlegChibikov.ZendeskInterview.Marketplace.DAL;

namespace OlegChibikov.ZendeskInterview.Marketplace.Tests
{
    public class TicketRelatedEntitiesLoaderTests
    {
        [Test]
        public void SetsOrganization()
        {
            // Arrange
            const int organizationId = 99;
            var ticket = new Ticket { OrganizationId = organizationId };
            var organization = new Organization();
            var organizationRepositoryMock = new Mock<ILiteDbRepository<Organization>>();
            organizationRepositoryMock.Setup(x => x.FindById(organizationId)).Returns(organization);
            var sut = new TicketRelatedEntitiesLoader(organizationRepositoryMock.Object, Mock.Of<ILiteDbRepository<User>>(), Mock.Of<IRelatedEntitiesLoader>());

            // Act
            sut.LoadRelatedEntities(ticket);

            // Assert
            Assert.That(ticket.Organization, Is.EqualTo(organization));
        }

        [Test]
        public void SetsAssignee()
        {
            // Arrange
            const int assigneeId = 11;
            var ticket = new Ticket { AssigneeId = assigneeId };
            var user = new User();
            var userRepositoryMock = new Mock<ILiteDbRepository<User>>();
            userRepositoryMock.Setup(x => x.FindById(assigneeId)).Returns(user);
            var sut = new TicketRelatedEntitiesLoader(Mock.Of<ILiteDbRepository<Organization>>(), userRepositoryMock.Object, Mock.Of<IRelatedEntitiesLoader>());

            // Act
            sut.LoadRelatedEntities(ticket);

            // Assert
            Assert.That(ticket.Assignee, Is.EqualTo(user));
        }

        [Test]
        public void SetsSubmitter()
        {
            // Arrange
            const int submitterId = 11;
            var ticket = new Ticket { SubmitterId = submitterId };
            var user = new User();
            var userRepositoryMock = new Mock<ILiteDbRepository<User>>();
            userRepositoryMock.Setup(x => x.FindById(submitterId)).Returns(user);
            var sut = new TicketRelatedEntitiesLoader(Mock.Of<ILiteDbRepository<Organization>>(), userRepositoryMock.Object, Mock.Of<IRelatedEntitiesLoader>());

            // Act
            sut.LoadRelatedEntities(ticket);

            // Assert
            Assert.That(ticket.Submitter, Is.EqualTo(user));
        }

        [Test]
        public void SetsAssigneeRelatedEntities()
        {
            // Arrange
            const int assigneeId = 11;
            var ticket = new Ticket { AssigneeId = assigneeId };
            var user = new User();
            var userRepositoryMock = new Mock<ILiteDbRepository<User>>();
            userRepositoryMock.Setup(x => x.FindById(assigneeId)).Returns(user);
            var userRelatedEntitiesLoader = new Mock<IRelatedEntitiesLoader>();
            var sut = new TicketRelatedEntitiesLoader(Mock.Of<ILiteDbRepository<Organization>>(), userRepositoryMock.Object, userRelatedEntitiesLoader.Object);

            // Act
            sut.LoadRelatedEntities(ticket);

            // Assert
            userRelatedEntitiesLoader.Verify(x => x.LoadRelatedEntities(user), Times.Exactly(1));
        }

        [Test]
        public void SetsSubmitterRelatedEntities()
        {
            // Arrange
            const int submitterId = 11;
            var ticket = new Ticket { SubmitterId = submitterId };
            var user = new User();
            var userRepositoryMock = new Mock<ILiteDbRepository<User>>();
            userRepositoryMock.Setup(x => x.FindById(submitterId)).Returns(user);
            var userRelatedEntitiesLoader = new Mock<IRelatedEntitiesLoader>();
            var sut = new TicketRelatedEntitiesLoader(Mock.Of<ILiteDbRepository<Organization>>(), userRepositoryMock.Object, userRelatedEntitiesLoader.Object);

            // Act
            sut.LoadRelatedEntities(ticket);

            // Assert
            userRelatedEntitiesLoader.Verify(x => x.LoadRelatedEntities(user), Times.Exactly(1));
        }
    }
}
