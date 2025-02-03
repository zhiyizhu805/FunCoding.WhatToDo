using FluentAssertions;
using FunCoding.WhatToDo.WebApi.Controllers;
using FunCoding.WhatToDo.WebApi.Interfaces;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
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

}
