using System.Collections.Generic;
using System.Linq;
using EntityFramework.MoqHelper;
using MapGen.Model.Database.EDM;
using MapGen.Model.Database.Repository;
using Moq;
using NUnit.Framework;

namespace MapGen.Model.Tests.Database
{
    [TestFixture]
    public class MapsRepositoryTest
    {
        private List<Map> _maps;
        private Mock<MapGenEntities> _mockContext;

        [SetUp]
        public void Initialization()
        {
            _maps = new List<Map>()
            {
                new Map
                {
                    Idm = 0,
                    Name = "Name0",
                    Length = 100,
                    Width = 100,
                    Scale = 1000
                },
                new Map
                {
                    Idm = 1,
                    Name = "Name1",
                    Length = 100,
                    Width = 100,
                    Scale = 10000
                },
                new Map
                {
                    Idm = 2,
                    Name = "Name2",
                    Length = 100,
                    Width = 100,
                    Scale = 100000
                },
            };

            var mockSet = EntityFrameworkMoqHelper.CreateMockForDbSet<Map>()
                .SetupForQueryOn(_maps)
                .WithAdd(_maps, "Idm")
                .WithFind(_maps, "Idm")
                .WithRemove(_maps);
            _mockContext = EntityFrameworkMoqHelper.CreateMockForDbContext<MapGenEntities, Map>(mockSet);
            _mockContext.Setup(c => c.Maps).Returns(mockSet.Object);
        }

        [Test]
        public void MapsRepository_init_success()
        {
            // Arrange.
            // Act.
            MapsRepository mapsRepository = new MapsRepository(_mockContext.Object);

            // Assert.
            Assert.IsNotNull(mapsRepository);
        }

        [Test]
        public void MapsRepository_GetAll_init_3_elements_result_3_elements()
        {
            // Arrange.
            MapsRepository mapsRepository = new MapsRepository(_mockContext.Object);

            // Act.
            var maps = mapsRepository.GetAll();
            List<Map> getAll = new List<Map>();
            foreach (var el in maps)
            {
                getAll.Add(el);
            }

            // Assert.
            Assert.AreEqual(3, getAll.Count);
            Assert.IsNotNull(getAll.Find(el =>
                el.Idm == 1 && 
                el.Name == "Name1" &&
                el.Length == 100 &&
                el.Width == 100 &&
                el.Scale == 10000));
        }

        [Test]
        public void MapsRepository_Create_1_element_result_is_added()
        {
            // Arrange.
            MapsRepository mapsRepository = new MapsRepository(_mockContext.Object);
            Map addMap = new Map
            {
                Name = "added name",
                Length = 120,
                Width = 120,
                Scale = 1200
            };

            // Act.
            mapsRepository.Create(addMap);

            // Assert.
            Assert.AreEqual(1, _mockContext.Object.Maps.Where(el =>
                el.Name == "added name" &&
                el.Length == 120 &&
                el.Width == 120 &&
                el.Scale == 1200).ToList().Count);
        }

        [Test]
        public void MapsRepository_Delete_1_element_result_is_deleted()
        {
            // Arrange.
            MapsRepository mapsRepository = new MapsRepository(_mockContext.Object);

            // Act.
            mapsRepository.Delete(1);
            
            // Assert.
            Assert.AreEqual(0, _mockContext.Object.Maps.Where(el =>
                el.Name == "Name1" &&
                el.Length == 100 &&
                el.Width == 100 &&
                el.Scale == 10000).ToList().Count);
        }
    }
}
