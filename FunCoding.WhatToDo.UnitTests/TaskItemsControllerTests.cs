using FluentAssertions;
using FunCoding.WhatToDo.WebApi.Controllers;
using FunCoding.WhatToDo.WebApi.Interfaces;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace FunCoding.WhatToDo.UnitTests;

public class TaskItemsControllerTests
{
    [Fact]
    public async Task GetTaskItems_ShouldReturn200Ok_WithTaskList_WhenRepositoryReturnsData()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        var fakeTaskItems = new List<TaskItem>
        {
            new TaskItem
            {
                Title = "Fake title 1",
                Description = "Fake Description 1"
            },
            new TaskItem
            {
                Title = "Fake title 2",
                Description = "Fake Description 2"
            }
        };
        mockRepo.Setup(repo => repo.GetTaskItemsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(fakeTaskItems);
        var controller = new TaskItemsController(mockRepo.Object);

        //Act
        var result = await controller.GetTaskItems();
        //Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200);
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeOfType<List<TaskItem>>().Which.Should().HaveCount(fakeTaskItems.Count)
            .And.BeEquivalentTo(fakeTaskItems);
        mockRepo.Verify(repo => repo.GetTaskItemsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskItems_ShouldReturn200Ok_WithEmptyList_WhenRepositoryReturnsEmpty()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        mockRepo.Setup(repo => repo.GetTaskItemsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<TaskItem>());
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.GetTaskItems();
        //Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200);
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        ((List<TaskItem>) okResult.Value!).Should().BeEmpty();
    }

    [Fact]
    public async Task GetTaskItem_ShouldReturn200Ok_WithRepositoryFindsItem()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        var fakeTaskItem = new TaskItem
        {
            Title = "Fake task item",
            Description = "Fake task item"
        };
        mockRepo.Setup(repo => repo.GetTaskItemAsync(It.IsAny<Guid>())).ReturnsAsync(fakeTaskItem);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.GetTaskItem(Guid.Parse("798a4706-4c51-48c9-8310-531d7364c926"));
        //Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeOfType<TaskItem>().Which.Should().BeEquivalentTo(fakeTaskItem);

    }

    [Fact]
    public async Task GetTaskItem_ShouldReturn404NotFound_WhenRepositoryReturnsNull()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        mockRepo.Setup(repo => repo.GetTaskItemAsync(It.IsAny<Guid>())).ReturnsAsync((TaskItem) null!);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.GetTaskItem(Guid.Parse("798a4706-4c51-48c9-8310-531d7364c926"));
        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }
    [Fact]
    public async Task CreateTaskItem_ShouldReturn201Created_WithCreatedTaskItem_WhenRepositoryCreatesSuccessfully()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        var newTaskItem = new TaskItem
        {
            Title = "new task item",
            Description = "new task item"
        };
        mockRepo.Setup(repo => repo.CreateTaskItemAsync(It.IsAny<TaskItem>())).ReturnsAsync(newTaskItem);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.CreateTaskItem(newTaskItem);
        var createdResult = result as CreatedAtActionResult;
        //Assert
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeOfType<TaskItem>().Which.Should().BeEquivalentTo(newTaskItem, options => options.Excluding(t => t.Id).Excluding(t => t.CreatedAt));
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturn200Ok_WithUpdatedTaskItem_WhenRepositoryUpdatedSuccessfully()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        var updatedTaskItem = new TaskItem
        {
            Title = "updated task item",
            Description = "updated task item"
        };
        mockRepo.Setup(repo => repo.UpdateTaskItemAsync(updatedTaskItem.Id, updatedTaskItem))
            .ReturnsAsync(updatedTaskItem);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.UpdateTaskItem(updatedTaskItem.Id, updatedTaskItem);
        var okResult = result as OkObjectResult;
        //Assert
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeOfType<TaskItem>().Which.Should().BeEquivalentTo(updatedTaskItem);
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturn404NotFound_WhenRepositoryReturnsNull()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        mockRepo.Setup(repo => repo.UpdateTaskItemAsync(It.IsAny<Guid>(), new TaskItem { Title = "Test task" }))
            .ReturnsAsync((TaskItem) null!);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.UpdateTaskItem(It.IsAny<Guid>(), new TaskItem { Title = "Test task" });
        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteTaskItem_ShouldReturn200Ok_WhenRepositoryDeletesSuccessfully()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        var taskItem = new TaskItem
        {
            Title = "task item",
            Description = "task item"
        };
        mockRepo.Setup(repo => repo.DeleteTaskItemAsync(taskItem.Id)).ReturnsAsync(taskItem);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.DeleteTaskItem(taskItem.Id);
        var okResult = result as OkResult;
        //Assert
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturn404NotFound_WhenRepositoryReturnsNull()
    {
        //Arrange
        var mockRepo = new Mock<ITaskItemRepository>();
        mockRepo.Setup(repo => repo.DeleteTaskItemAsync(It.IsAny<Guid>())).ReturnsAsync((TaskItem) null!);
        var controller = new TaskItemsController(mockRepo.Object);
        //Act
        var result = await controller.DeleteTaskItem(It.IsAny<Guid>());
        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }



}
