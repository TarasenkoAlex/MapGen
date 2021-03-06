﻿using System;
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
    public class PointsRepositoryTest
    {
        private List<Point> _points;
        private Mock<MapGenEntities> _mockContext;

        [SetUp]
        public void Initialization()
        {
            _points = new List<Point>()
            {
                new Point
                {
                    Idp = 0,
                    X = 0,
                    Y = 1,
                    Depth = 1500,
                    Idm = 0
                },
                new Point
                {
                    Idp = 1,
                    X = 1,
                    Y = 1,
                    Depth = 2000,
                    Idm = 1
                },
                new Point
                {
                    Idp = 2,
                    X = 1,
                    Y = 2,
                    Depth = 1500,
                    Idm = 0
                },
            };

            var mockSet = EntityFrameworkMoqHelper.CreateMockForDbSet<Point>()
                .SetupForQueryOn(_points)
                .WithAdd(_points, "Idp")
                .WithFind(_points, "Idp")
                .WithRemove(_points);
            _mockContext = EntityFrameworkMoqHelper.CreateMockForDbContext<MapGenEntities, Point>(mockSet);
            _mockContext.Setup(c => c.Points).Returns(mockSet.Object);
        }

        [Test]
        public void PointsRepository_init_success()
        {
            // Arrange.
            // Act.
            PointsRepository pointsRepository = new PointsRepository(_mockContext.Object);

            // Assert.
            Assert.IsNotNull(pointsRepository);
        }

        [Test]
        public void PointsRepository_GetAll_init_3_elements_result_3_elements()
        {
            // Arrange.
            PointsRepository pointsRepository = new PointsRepository(_mockContext.Object);

            // Act.
            var points = pointsRepository.GetAll();
            List<Point> getAll = new List<Point>();
            foreach (var el in points)
            {
                getAll.Add(el);
            }

            // Assert.
            Assert.AreEqual(3, getAll.Count);
            Assert.IsNotNull(getAll.Find(el =>
                el.Idp == 1 &&
                Math.Abs(el.X - 1.0) < double.Epsilon &&
                Math.Abs(el.Y - 1.0) < double.Epsilon &&
                Math.Abs(el.Depth - 2000) < double.Epsilon &&
                el.Idm == 1));
        }

        [Test]
        public void PointsRepository_Create_1_element_result_is_added()
        {
            // Arrange.
            PointsRepository pointsRepository = new PointsRepository(_mockContext.Object);
            Point addPoint = new Point
            {
                X = 5,
                Y = 2,
                Depth = 5000,
                Idm = 1
            };

            // Act.
            pointsRepository.Create(addPoint);

            // Assert.
            Assert.AreEqual(1, _mockContext.Object.Points.Where(el =>
                el.X == 5 &&
                el.Y == 2 &&
                Math.Abs(el.Depth - 5000) < double.Epsilon &&
                el.Idm == 1).ToList().Count);
        }

        [Test]
        public void PointsRepository_Delete_1_element_result_is_deleted()
        {
            // Arrange.
            PointsRepository pointsRepository = new PointsRepository(_mockContext.Object);

            // Act.
            pointsRepository.Delete(1);
            
            // Assert.
            Assert.AreEqual(0, _mockContext.Object.Points.Where(el =>
                el.Idp == 1 &&
                el.X == 1 &&
                el.Y == 1 &&
                Math.Abs(el.Depth - 2000) < double.Epsilon &&
                el.Idm == 1).ToList().Count);
        }
    }
}
