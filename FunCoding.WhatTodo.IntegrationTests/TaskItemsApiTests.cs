using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using FunCoding.WhatTodo.IntegrationTests.Helpers;
using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FunCoding.WhatTodo.IntegrationTests;

public class TaskItemsApiTests(CustomIntegrationTestsFixture factory) : IClassFixture<CustomIntegrationTestsFixture>
{
    [Fact]
    public async Task GetTaskItems_ReturnsSuccessAndCorrectContentType()
    {
        //Arrange
        var client = factory.CreateClient();
        //Act
        var response = await client.GetAsync("api/taskItems");
        //Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        //Deserialize the response
        var responseContent = await response.Content.ReadAsStringAsync();
        var taskItems = JsonSerializer.Deserialize<List<TaskItem>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        taskItems.Should().NotBeNull();
        taskItems.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("8a9de219-2dde-4f2a-9ebd-b1f8df9fef03")]
    [InlineData("798a4706-4c51-48c9-8310-531d7364c926")]
    public async Task GetTaskById_ReturnTask_WhenTaskExists(Guid id)
    {
        //Arrange
        var client = factory.CreateClient();
        //Act
        var response = await client.GetAsync($"api/taskItems/{id}");
        //Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        //Deserialize the response
        var responseContent = await response.Content.ReadAsStringAsync();
        var taskItem = JsonSerializer.Deserialize<TaskItem>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        taskItem.Should().NotBeNull();
        taskItem.Id.Should().Be(id);

    }

    [Fact]
    public async Task GetTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        //Arrange
        var client = factory.CreateClient();
        //Act
        var response = await client.GetAsync($"api/taskItems/{Guid.NewGuid()}");
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedTask_WhenValidInput()
    {
        //Arrange
        var client = factory.CreateClient();
        var testTaskItem = new TaskItem
        {
            Title = "test task title",
            Description = "test task description"
        };
        var json = JsonSerializer.Serialize(testTaskItem);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        //Act
        var response = await client.PostAsync("/api/taskItems", data);
        //Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        //Deserialize
        var responseContent = await response.Content.ReadAsStringAsync();
        var taskItem = JsonSerializer.Deserialize<TaskItem>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        taskItem.Should().NotBeNull();
        taskItem.Id.Should().NotBeEmpty();
        taskItem.Title.Should().Be(testTaskItem.Title);
        taskItem.Description.Should().Be(testTaskItem.Description);

        //Clean up the database
        var scope = factory.Services.CreateScope();
        var scopedService = scope.ServiceProvider;
        var db = scopedService.GetRequiredService<ApplicationDbContext>();
        Utilities.Cleanup(db);
    }

    [Fact]
    public async Task UpdateTask_ReturnUpdatedTask_WhenTaskExisted()
    {
        //Arrange
        var client = factory.CreateClient();
        var id = Guid.Parse("8a9de219-2dde-4f2a-9ebd-b1f8df9fef03");
        var testUpdateTaskItem = new TaskItem
        {
            Title = "test title",
            Description = "test description",
        };
        var json = JsonSerializer.Serialize(testUpdateTaskItem);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        //Act
        var response = await client.PutAsync($"/api/taskItems/{id}", data);
        //Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        //Deserialize
        var responseContent = await response.Content.ReadAsStringAsync();
        var taskItem = JsonSerializer.Deserialize<TaskItem>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        taskItem.Should().NotBeNull();
        taskItem.Id.Should().NotBeEmpty();
        taskItem.Title.Should().Be(testUpdateTaskItem.Title);
        taskItem.Description.Should().Be(testUpdateTaskItem.Description);
        //Clean up the database
        var scope = factory.Services.CreateScope();
        var scopedService = scope.ServiceProvider;
        var db = scopedService.GetRequiredService<ApplicationDbContext>();
        Utilities.Cleanup(db);

    }

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        //Arrange
        var client = factory.CreateClient();
        var id = Guid.NewGuid();
        var testUpdateTaskItem = new TaskItem
        {
            Title = "test title",
            Description = "test description",
        };
        var json = JsonSerializer.Serialize(testUpdateTaskItem);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        //Act
        var response = await client.PutAsync($"/api/taskItems/{id}", data);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTaskById_ReturnsOk_WhenTaskExists()
    {
        //Arrange
        var client = factory.CreateClient();
        var id = Guid.Parse("8a9de219-2dde-4f2a-9ebd-b1f8df9fef03");
        //Act
        var response = await client.DeleteAsync($"/api/taskItems/{id}");
        //Assert
        response.EnsureSuccessStatusCode();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deletedTask = await db.TaskItems.FindAsync(id);
            deletedTask.Should().BeNull();
            Utilities.Cleanup(db);
        }
    }

    [Fact]
    public async Task DeleteTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        //Arrange
        var client = factory.CreateClient();
        var id = Guid.NewGuid();
        //Act
        var response = await client.DeleteAsync($"/api/taskItems/{id}");
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }



}
