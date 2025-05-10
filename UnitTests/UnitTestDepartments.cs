using NUnit.Framework;
using Moq;
using SchedlifyApi.Controllers;
using SchedlifyApi.DTO;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTests;

[TestFixture]
public class DepartmentsControllerTests
{
    private Mock<IDepartmentRepository> _departmentRepo = null!;
    private Mock<IUniversityRepository> _universityRepo = null!;
    private DepartmentsController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _departmentRepo = new Mock<IDepartmentRepository>();
        _universityRepo = new Mock<IUniversityRepository>();
        _controller = new DepartmentsController(_departmentRepo.Object, _universityRepo.Object);
    }

    [Test]
    public async Task GetDepartments_ReturnsDepartmentsList()
    {
        // Arrange
        var mockDepartments = new List<Department>
        {
            new Department { Id = 1, Name = "CS", UniversityId = 1 },
            new Department { Id = 2, Name = "Math", UniversityId = 1 }
        };

        _departmentRepo
            .Setup(repo => repo.GetAll(1, null, 0, 10))
            .ReturnsAsync(mockDepartments);

        // Act
        var result = await _controller.GetDepartments(universityId: 1, s: null);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = (OkObjectResult)result.Result!;
        var departments = okResult.Value as List<DepartmentResponse>;
        Assert.NotNull(departments);
        Assert.AreEqual(2, departments!.Count);
        Assert.AreEqual("CS", departments[0].Name);
    }

    [Test]
    public async Task GetDepartmentById_ReturnsNotFound_WhenDepartmentMissing()
    {
        // Arrange
        _departmentRepo
            .Setup(repo => repo.GetById(99))
            .ReturnsAsync((Department?)null);

        // Act
        var result = await _controller.GetDepartmentById(99);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    [Test]
    public async Task GetDepartmentById_ReturnsDepartment_WhenExists()
    {
        // Arrange
        var department = new Department { Id = 2, Name = "Physics", UniversityId = 1 };
        _departmentRepo
            .Setup(repo => repo.GetById(2))
            .ReturnsAsync(department);

        // Act
        var result = await _controller.GetDepartmentById(2);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var ok = (OkObjectResult)result.Result!;
        var response = ok.Value as DepartmentResponse;
        Assert.NotNull(response);
        Assert.AreEqual("Physics", response!.Name);
    }

    [Test]
    public async Task CreateDepartment_ReturnsBadRequest_WhenUniversityMissing()
    {
        // Arrange
        var request = new CreateDepartmentRequest
        {
            Name = "Engineering",
            UniversityId = 123 // non-existing
        };

        _universityRepo
            .Setup(repo => repo.GetById(123))
            .ReturnsAsync((University?)null);

        // Act
        var result = await _controller.CreateDepartment(request);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
    }

    [Test]
    public async Task CreateDepartment_ReturnsCreated_WhenValid()
    {
        // Arrange
        var request = new CreateDepartmentRequest
        {
            Name = "Engineering",
            UniversityId = 1
        };

        var university = new University { Id = 1, Name = "Test University" };
        var newDepartment = new Department { Id = 10, Name = request.Name, UniversityId = request.UniversityId };

        _universityRepo
            .Setup(repo => repo.GetById(1))
            .ReturnsAsync(university);

        _departmentRepo
            .Setup(repo => repo.Create(It.IsAny<Department>()))
            .ReturnsAsync(newDepartment);

        // Act
        var result = await _controller.CreateDepartment(request);

        // Assert
        Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
        var created = (CreatedAtActionResult)result.Result!;
        var response = created.Value as DepartmentResponse;
        Assert.NotNull(response);
        Assert.AreEqual(10, response!.Id);
        Assert.AreEqual("Engineering", response.Name);
    }
}
