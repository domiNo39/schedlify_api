using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SchedlifyApi.Controllers;
using SchedlifyApi.DTO;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;

namespace SchedlifyApi.Tests.Controllers
{
    public class GroupsControllerTests
    {
        private Mock<IGroupRepository> _groupRepoMock;
        private Mock<IDepartmentRepository> _departmentRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private GroupsController _controller;

        [SetUp]
        public void Setup()
        {
            _groupRepoMock = new Mock<IGroupRepository>();
            _departmentRepoMock = new Mock<IDepartmentRepository>();
            _userRepoMock = new Mock<IUserRepository>();

            _controller = new GroupsController(
                _groupRepoMock.Object,
                _departmentRepoMock.Object,
                _userRepoMock.Object
            );
        }

        [Test]
        public async Task GetGroups_ReturnsListOfGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group A", DepartmentId = 10, AdministratorId = 100 },
                new Group { Id = 2, Name = "Group B", DepartmentId = 10, AdministratorId = 101 }
            };

            _groupRepoMock
                .Setup(r => r.GetAll(null, null, null, 0, 10))
                .ReturnsAsync(groups);

            // Act
            var result = await _controller.GetGroups();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedGroups = okResult.Value as List<GroupResponse>;
            Assert.AreEqual(2, returnedGroups.Count);
            Assert.AreEqual("Group A", returnedGroups[0].Name);
        }

        [Test]
        public async Task GetGroupById_ReturnsGroupExtendedResponse()
        {
            // Arrange
            var group = new Group
            {
                Id = 1,
                Name = "Group A",
                DepartmentId = 10,
                AdministratorId = 100,
                Department = new Department
                {
                    Id = 10,
                    Name = "Dep A",
                    UniversityId = 1,
                    University = new University
                    {
                        Id = 1,
                        Name = "Uni A"
                    }
                }
            };

            _groupRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(group);

            // Act
            var result = await _controller.GetGroupById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedGroup = okResult.Value as GroupExtendedResponse;
            Assert.AreEqual("Group A", returnedGroup.Name);
            Assert.AreEqual("Dep A", returnedGroup.Department.Name);
            Assert.AreEqual("Uni A", returnedGroup.University.Name);
        }

        [Test]
        public async Task GetGroupById_ReturnsBadRequest_IfNotFound()
        {
            _groupRepoMock.Setup(r => r.GetById(99)).ReturnsAsync((Group?)null);

            var result = await _controller.GetGroupById(99);

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
            Assert.AreEqual("Group does not exist.", badRequest.Value);
        }

        [Test]
        public async Task CreateGroup_ReturnsCreatedGroup()
        {
            var request = new CreateGroupRequest
            {
                Name = "Group A",
                DepartmentId = 10
            };

            var department = new Department { Id = 10, Name = "Dep A" };
            var user = new User { Id = 100, Login = "Alice" };

            _departmentRepoMock.Setup(r => r.GetById(10)).ReturnsAsync(department);
            _userRepoMock.Setup(r => r.GetUserById(100)).ReturnsAsync(user);

            var createdGroup = new Group
            {
                Id = 1,
                Name = "Group A",
                DepartmentId = 10,
                AdministratorId = 100
            };

            _groupRepoMock.Setup(r => r.Create(It.IsAny<Group>())).ReturnsAsync(createdGroup);

            var result = await _controller.CreateGroup(request, 100);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            var response = createdResult.Value as GroupResponse;
            Assert.AreEqual(1, response.Id);
            Assert.AreEqual("Group A", response.Name);
            Assert.AreEqual(100, response.AdministratorId);
        }

        [Test]
        public async Task CreateGroup_ReturnsBadRequest_IfDepartmentMissing()
        {
            var request = new CreateGroupRequest
            {
                Name = "Group A",
                DepartmentId = 10
            };

            _departmentRepoMock.Setup(r => r.GetById(10)).ReturnsAsync((Department?)null);

            var result = await _controller.CreateGroup(request, 100);

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
            Assert.AreEqual("Department does not exist.", badRequest.Value);
        }

        [Test]
        public async Task CreateGroup_ReturnsBadRequest_IfUserMissing()
        {
            var request = new CreateGroupRequest
            {
                Name = "Group A",
                DepartmentId = 10
            };

            _departmentRepoMock.Setup(r => r.GetById(10)).ReturnsAsync(new Department { Id = 10 });
            _userRepoMock.Setup(r => r.GetUserById(100)).ReturnsAsync((User?)null);

            var result = await _controller.CreateGroup(request, 100);

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
            Assert.AreEqual("User does not exist.", badRequest.Value);
        }
    }
}
