using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SchedlifyApi.Controllers;
using SchedlifyApi.DTO;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;
using System;
using System.Threading.Tasks;

[TestFixture]
public class TgUsersControllerTests
{
    private Mock<ITgUserRepository> _mockRepo;
    private TgUsersController _controller;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITgUserRepository>();
        _controller = new TgUsersController(_mockRepo.Object);
    }

    [Test]
    public async Task CreateTgUser_ReturnsConflict_IfUserAlreadyExists()
    {
        var telegramUid = 12345L;
        var request = new CreateTgUserRequest { Username = "existing", FirstName = "Test" };

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync(new TgUser());

        var result = await _controller.CreateTgUser(telegramUid, request);

        Assert.IsInstanceOf<ConflictObjectResult>(result.Result);
    }

    [Test]
    public async Task CreateTgUser_ReturnsConflict_IfUsernameAlreadyExists()
    {
        var telegramUid = 12345L;
        var request = new CreateTgUserRequest { Username = "duplicate", FirstName = "Test" };

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync((TgUser)null);
        _mockRepo.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(new TgUser());

        var result = await _controller.CreateTgUser(telegramUid, request);

        Assert.IsInstanceOf<ConflictObjectResult>(result.Result);
    }

    [Test]
    public async Task CreateTgUser_ReturnsCreatedUser()
    {
        var telegramUid = 12345L;
        var request = new CreateTgUserRequest { Username = "unique", FirstName = "John" };

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync((TgUser)null);
        _mockRepo.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((TgUser)null);

        var newUser = new TgUser
        {
            Id = telegramUid,
            Username = "unique",
            FirstName = "John",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<TgUser>())).ReturnsAsync(newUser);

        var result = await _controller.CreateTgUser(telegramUid, request);

        var createdAt = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdAt);
        Assert.IsInstanceOf<TgUserResponse>(createdAt.Value);
    }

    [Test]
    public async Task GetTgUser_ReturnsNotFound_IfUserDoesNotExist()
    {
        long telegramUid = 123;

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync((TgUser)null);

        var result = await _controller.GetTgUser(telegramUid);

        Assert.IsInstanceOf<NotFoundResult>(result.Result);
    }

    [Test]
    public async Task GetTgUser_ReturnsUser_IfExists()
    {
        long telegramUid = 123;

        var user = new TgUser
        {
            Id = telegramUid,
            FirstName = "John",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync(user);

        var result = await _controller.GetTgUser(telegramUid);

        Assert.IsInstanceOf<TgUserResponse>(result.Value);
        Assert.AreEqual(telegramUid, result.Value.Id);
    }

    [Test]
    public async Task SelectGroup_ReturnsNotFound_IfUserDoesNotExist()
    {
        long telegramUid = 123;
        int groupId = 1;

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync((TgUser)null);

        var result = await _controller.SelectGroup(telegramUid, groupId);

        Assert.IsInstanceOf<NotFoundResult>(result.Result);
    }

    [Test]
    public async Task SelectGroup_UpdatesGroup_AndReturnsUser()
    {
        long telegramUid = 123;
        int groupId = 1;

        var user = new TgUser
        {
            Id = telegramUid,
            FirstName = "Test",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync(user);

        var result = await _controller.SelectGroup(telegramUid, groupId);

        _mockRepo.Verify(r => r.UpdateAsync(user), Times.Once);
        Assert.AreEqual(groupId, result.Value.GroupId);
    }

    [Test]
    public async Task ChangeSubscription_TogglesSubscribedFlag()
    {
        long telegramUid = 123;

        var user = new TgUser
        {
            Id = telegramUid,
            FirstName = "John",
            CreatedAt = DateTime.UtcNow,
            Subscribed = false
        };

        _mockRepo.Setup(r => r.GetByIdAsync(telegramUid)).ReturnsAsync(user);

        var result = await _controller.SelectGroup(telegramUid); // calls change_subscription_status

        _mockRepo.Verify(r => r.UpdateAsync(user), Times.Once);
        Assert.IsTrue(result.Value.Subscribed);
    }
}
