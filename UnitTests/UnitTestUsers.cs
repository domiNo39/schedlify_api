using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using SchedlifyApi.Controllers;
using SchedlifyApi.Repositories;
using SchedlifyApi.Models;
using SchedlifyApi.DTO;

namespace SchedlifyApi.Tests.Controllers
{
    public class UsersControllerTests
    {
        private UsersController _controller;
        private Mock<IUserRepository> _repositoryMock;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _controller = new UsersController(_repositoryMock.Object);
        }

        [Test]
        public async Task Register_UserAlreadyExists_ReturnsConflict()
        {
            var request = new RegisterUserRequest { Login = "testuser", Password = "pass123" };
            _repositoryMock.Setup(r => r.GetByLogin("testuser")).Returns(new User());

            var result = await _controller.Register(request);

            Assert.IsInstanceOf<ConflictObjectResult>(result.Result);
        }

        [Test]
        public async Task Register_NewUser_ReturnsCreatedUser()
        {
            var request = new RegisterUserRequest { Login = "newuser", Password = "password" };
            _repositoryMock.Setup(r => r.GetByLogin("newuser")).Returns((User)null!);
            _repositoryMock.Setup(r => r.Add(It.IsAny<User>())).Returns((User u) =>
            {
                u.Id = 1;
                return u;
            });

            var result = await _controller.Register(request);
            var createdAt = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(createdAt);
            Assert.IsInstanceOf<UserResponse>(createdAt.Value);
            Assert.AreEqual(1, ((UserResponse)createdAt.Value!).Id);
        }

        [Test]
        public void Login_InvalidLogin_ReturnsUnauthorized()
        {
            var request = new LoginUserRequest { Login = "unknown", Password = "1234" };
            _repositoryMock.Setup(r => r.GetByLogin("unknown")).Returns((User)null!);

            var result = _controller.Login(request);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result.Result);
        }

        [Test]
        public void Login_WrongPassword_ReturnsUnauthorized()
        {
            var request = new LoginUserRequest { Login = "user", Password = "wrongpass" };
            var user = new User { Login = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass") };

            _repositoryMock.Setup(r => r.GetByLogin("user")).Returns(user);

            var result = _controller.Login(request);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result.Result);
        }

        [Test]
        public void Login_ValidCredentials_ReturnsUser()
        {
            var request = new LoginUserRequest { Login = "user", Password = "pass123" };
            var hashed = BCrypt.Net.BCrypt.HashPassword("pass123");
            var user = new User { Id = 2, Login = "user", PasswordHash = hashed };

            _repositoryMock.Setup(r => r.GetByLogin("user")).Returns(user);

            var result = _controller.Login(request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            var userResponse = ok.Value as UserResponse;
            Assert.AreEqual(2, userResponse?.Id);
        }

        [Test]
        public async Task GetUserById_NotFound_ReturnsNotFound()
        {
            _repositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync((User)null!);

            var result = await _controller.GetUserById(1);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetUserById_Found_ReturnsUser()
        {
            var user = new User { Id = 1, Login = "user" };
            _repositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

            var result = await _controller.GetUserById(1);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var response = (result.Result as OkObjectResult)?.Value as UserResponse;
            Assert.AreEqual("user", response?.Login);
        }
    }
}
