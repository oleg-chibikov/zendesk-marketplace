using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts.Data;
using OlegChibikov.ZendeskInterview.Marketplace.Core;

namespace OlegChibikov.ZendeskInterview.Marketplace.Tests
{
    public class SearchRequestProcessorTests
    {
        [Test]
        public void WaitsForUserInput()
        {
            // Arrange
            var (userInputReceiverMock, entityType, _, _) = SetupUserInputReceiver();
            var sut = new SearchRequestProcessor(userInputReceiverMock.Object, new Dictionary<Type, IEntitySearcher> { { entityType, Mock.Of<IEntitySearcher>() } }, Mock.Of<IOutputFormatter>());

            // Act
            sut.ProcessSearchRequest();

            // Assert
            userInputReceiverMock.VerifyAll();
        }

        [Test]
        public void CallsEntitySearcherWithInputParameters()
        {
            // Arrange
            var entitySearcherMock = new Mock<IEntitySearcher>();
            var (userInputReceiverMock, entityType, propertyInfo, value) = SetupUserInputReceiver();
            entitySearcherMock.Setup(x => x.Find(propertyInfo.Name, value, false));
            var sut = new SearchRequestProcessor(userInputReceiverMock.Object, new Dictionary<Type, IEntitySearcher> { { entityType, entitySearcherMock.Object } }, Mock.Of<IOutputFormatter>());

            // Act
            sut.ProcessSearchRequest();

            // Assert
            entitySearcherMock.VerifyAll();
        }

        [Test]
        public void DoesNotCallOutputFormatter_When_NoDataIsFound()
        {
            // Arrange
            var outputFormatterMock = new Mock<IOutputFormatter>();
            var (userInputReceiverMock, entityType, _, _) = SetupUserInputReceiver();
            var sut = new SearchRequestProcessor(userInputReceiverMock.Object, new Dictionary<Type, IEntitySearcher> { { entityType, Mock.Of<IEntitySearcher>() } }, outputFormatterMock.Object);

            // Act
            var result = sut.ProcessSearchRequest();

            // Assert
            Assert.That(result, Is.Null);
            outputFormatterMock.Verify(x => x.ListProperties(It.IsAny<object?>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void CallsOutputFormatterForEachOrganization_And_MergesTheResults_When_DataIsFound()
        {
            // Arrange
            var entitySearcherMock = new Mock<IEntitySearcher>();
            var (userInputReceiverMock, entityType, propertyInfo, value) = SetupUserInputReceiver();
            var organization1 = new Organization();
            var organization2 = new Organization();
            entitySearcherMock.Setup(x => x.Find(propertyInfo.Name, value, false)).Returns(new[] { organization1, organization2 });
            var outputFormatterMock = new Mock<IOutputFormatter>();
            const string? organization1Output = "1";
            const string? organization2Output = "2";
            outputFormatterMock.SetupSequence(x => x.ListProperties(organization1, It.IsAny<int>())).Returns(organization1Output);
            outputFormatterMock.SetupSequence(x => x.ListProperties(organization2, It.IsAny<int>())).Returns(organization2Output);
            var sut = new SearchRequestProcessor(userInputReceiverMock.Object, new Dictionary<Type, IEntitySearcher> { { entityType, entitySearcherMock.Object } }, outputFormatterMock.Object);

            // Act
            var result = sut.ProcessSearchRequest();

            // Assert
            outputFormatterMock.VerifyAll();
            Assert.That(result, Does.Contain(organization1Output));
            Assert.That(result, Does.Contain(SearchRequestProcessor.Separator));
            Assert.That(result, Does.Contain(organization2Output));
        }

        static (Mock<IUserInputReceiver>, Type, PropertyInfo, int) SetupUserInputReceiver()
        {
            var userInputReceiverMock = new Mock<IUserInputReceiver>();
            var entityType = typeof(Organization);
            var propertyInfo = entityType.GetProperty("Id") !;
            const int value = 1;
            userInputReceiverMock.Setup(x => x.WaitForEntityType()).Returns(entityType);
            userInputReceiverMock.Setup(x => x.WaitForPropertyInfo(entityType)).Returns(propertyInfo);
            userInputReceiverMock.Setup(x => x.WaitForSearchValue(propertyInfo)).Returns(value);
            return (userInputReceiverMock, entityType, propertyInfo, value);
        }
    }
}