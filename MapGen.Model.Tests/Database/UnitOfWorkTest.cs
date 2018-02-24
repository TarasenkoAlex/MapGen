using System;
using MapGen.Model.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MapGen.Model.Tests.Database
{
    [TestClass]
    public class UnitOfWorkTest
    {
        [TestMethod]
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
