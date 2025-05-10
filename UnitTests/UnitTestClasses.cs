using Moq;
using NUnit.Framework;
using SchedlifyApi.Controllers;
using SchedlifyApi.DTO;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchedlifyApi.Tests.Controllers
{
    public class ClassesControllerTests
    {
        private Mock<IClassRepository> _mockClassRepository;
        private Mock<IGroupRepository> _mockGroupRepository;
        private ClassesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockClassRepository = new Mock<IClassRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _controller = new ClassesController(_mockClassRepository.Object, _mockGroupRepository.Object);
        }

        [Test]
        public async Task GetClassesByGroupId_GroupNotFound_ReturnsBadRequest()
        {
            // Arrange
            var groupId = 1;
            _mockGroupRepository.Setup(r => r.GetById(groupId)).ReturnsAsync((Group)null);

            // Act
            var result = await _controller.GetClassesByGroupId(groupId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public async Task GetClassesByGroupId_ClassesFound_ReturnsOk()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { Id = groupId };
            var classes = new List<Class>
            {
                new Class { Id = 1, Name = "Class 1", GroupId = groupId },
                new Class { Id = 2, Name = "Class 2", GroupId = groupId }
            };

            _mockGroupRepository.Setup(r => r.GetById(groupId)).ReturnsAsync(group);
            _mockClassRepository.Setup(r => r.GetByGroupId(groupId)).ReturnsAsync(classes);

            // Act
            var result = await _controller.GetClassesByGroupId(groupId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as List<ClassResponse>;
            Assert.AreEqual(2, response.Count);
        }

        [Test]
        public async Task GetClassById_ClassNotFound_ReturnsNotFound()
        {
            // Arrange
            var classId = 1;
            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync((Class)null);

            // Act
            var result = await _controller.GetClassById(classId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public async Task GetClassById_ClassFound_ReturnsOk()
        {
            // Arrange
            var classId = 1;
            var class_ = new Class { Id = classId, Name = "Class 1", GroupId = 1 };
            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync(class_);

            // Act
            var result = await _controller.GetClassById(classId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ClassResponse;
            Assert.AreEqual(classId, response.Id);
            Assert.AreEqual("Class 1", response.Name);
        }

        [Test]
        public async Task CreateClass_GroupNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateClassRequest { Name = "New Class", GroupId = 1 };
            _mockGroupRepository.Setup(r => r.GetById(request.GroupId)).ReturnsAsync((Group)null);

            // Act
            var result = await _controller.CreateClass(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public async Task CreateClass_ClassCreated_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new CreateClassRequest { Name = "New Class", GroupId = 1 };
            var group = new Group { Id = 1 };
            var createdClass = new Class { Id = 1, Name = "New Class", GroupId = 1 };

            _mockGroupRepository.Setup(r => r.GetById(request.GroupId)).ReturnsAsync(group);
            _mockClassRepository.Setup(r => r.Create(It.IsAny<Class>())).ReturnsAsync(createdClass);

            // Act
            var result = await _controller.CreateClass(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var response = createdResult.Value as ClassResponse;
            Assert.AreEqual(createdClass.Id, response.Id);
            Assert.AreEqual("New Class", response.Name);
        }

        [Test]
        public async Task EditClass_ClassNotFound_ReturnsNotFound()
        {
            // Arrange
            var classId = 1;
            var request = new EditClassRequest { Name = "Updated Class", GroupId = 1 };
            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync((Class)null);

            // Act
            var result = await _controller.EditClass(classId, request);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task EditClass_GroupNotFound_ReturnsBadRequest()
        {
            // Arrange
            var classId = 1;
            var request = new EditClassRequest { Name = "Updated Class", GroupId = 1 };
            var class_ = new Class { Id = classId, Name = "Class 1", GroupId = 1 };

            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync(class_);
            _mockGroupRepository.Setup(r => r.GetById(request.GroupId)).ReturnsAsync((Group)null);

            // Act
            var result = await _controller.EditClass(classId, request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public async Task EditClass_ClassUpdated_ReturnsOk()
        {
            // Arrange
            var classId = 1;
            var request = new EditClassRequest { Name = "Updated Class", GroupId = 1 };
            var class_ = new Class { Id = classId, Name = "Class 1", GroupId = 1 };
            var group = new Group { Id = 1 };
            var updatedClass = new Class { Id = classId, Name = "Updated Class", GroupId = 1 };

            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync(class_);
            _mockGroupRepository.Setup(r => r.GetById(request.GroupId)).ReturnsAsync(group);
            _mockClassRepository.Setup(r => r.Update(It.IsAny<Class>())).ReturnsAsync(updatedClass);

            // Act
            var result = await _controller.EditClass(classId, request);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ClassResponse;
            Assert.AreEqual("Updated Class", response.Name);
        }

        [Test]
        public async Task DeleteClass_ClassNotFound_ReturnsNotFound()
        {
            // Arrange
            var classId = 1;
            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync((Class)null);

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DeleteClass_ClassDeleted_ReturnsNoContent()
        {
            // Arrange
            var classId = 1;
            var class_ = new Class { Id = classId, Name = "Class 1", GroupId = 1 };

            _mockClassRepository.Setup(r => r.GetByIdAsync(classId)).ReturnsAsync(class_);
            _mockClassRepository.Setup(r => r.Delete(classId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteClass(classId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
