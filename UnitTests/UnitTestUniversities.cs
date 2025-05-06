using NUnit.Framework;
using Moq;
using SchedlifyApi.Controllers;
using SchedlifyApi.Models;
using SchedlifyApi.DTO;
using SchedlifyApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests;

[TestFixture]
public class UniversitiesControllerTests
{
    private Mock<IUniversityRepository> _repositoryMock = null!;
    private UniversitiesController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IUniversityRepository>();
        _controller = new UniversitiesController(_repositoryMock.Object);
    }

    [Test]
    public async Task GetUniversities_ReturnsOkWithData()
    {
        // Arrange
        var mockUniversities = new List<University>
        {
            new University { Id = 1, Name = "University A" },
            new University { Id = 2, Name = "University B" }
        };

        _repositoryMock
            .Setup(repo => repo.GetAll(null, 0, 10))
            .ReturnsAsync(mockUniversities);

        // Act
        var result = await _controller.GetUniversities();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = (OkObjectResult)result.Result!;
        var universities = okResult.Value as List<UniversityResponse>;

        Assert.NotNull(universities);
        Assert.AreEqual(2, universities!.Count);
        Assert.AreEqual("University A", universities[0].Name);
    }
}