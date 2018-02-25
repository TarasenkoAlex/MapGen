using System;
using MapGen.Model.Database;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MapGen.Model.Tests.Database
{
    [TestFixture]
    public class UnitOfWorkTest
    {
        [Test]
        public void UnitOfWork_init_success()
        {
            // Arrange.
            // Act.
            UnitOfWork unitOfWork = new UnitOfWork();

            // Assert.
            Assert.IsNotNull(unitOfWork);
            Assert.IsNotNull(unitOfWork.Maps);
            Assert.IsNotNull(unitOfWork.Points);
        }
    }
}
