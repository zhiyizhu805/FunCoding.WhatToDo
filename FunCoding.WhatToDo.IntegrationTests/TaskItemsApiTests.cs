using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using FunCoding.WhatToDo.IntegrationTests.Helpers;
using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace FunCoding.WhatToDo.IntegrationTests;

public class TaskItemsApiTests(CustomIntegrationTestsFixture factory) : IClassFixture<CustomIntegrationTestsFixture>
{
    [Theory]
    [InlineData(1, 5, 5)]
    [InlineData(2, 5, 3)]
    [InlineData(1, 10, 8)]
    public async Task GetTasks_ReturnPaginatedTasks(int pageIndex, int pageSize, int expectedCount)
    {
        //Arrange
        var client = factory.CreateClient();
        //Act
        var response = await client.GetAsync($"/api/taskItems?pageIndex={pageIndex}&pageSize={pageSize}");
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
        taskItems.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyList_WhenNoTasksExist()
    {
        //Arrange - Empty the seed data
        var scope = factory.Services.CreateScope();
        var scopedService = scope.ServiceProvider;
        var db = scopedService.GetRequiredService<ApplicationDbContext>();
        Utilities.Cleanup(db);
        var client = factory.CreateClient();
        //Act
        var response = await client.GetAsync("/api/taskItems");
        //Assert(Response validation)
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        //Deserialize response
        var responseContent = await response.Content.ReadAsStringAsync();
        var taskItems = JsonSerializer.Deserialize<List<TaskItem>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        //Ensure API response contains expected data
        taskItems.Should().NotBeNull();
        taskItems.Should().BeEmpty();
        taskItems.Should().HaveCount(0);
        taskItems.Should().NotBeNull().And.BeEmpty();
        Utilities.Reset(db);
    }

    [Theory]
    [InlineData("8a9de219-2dde-4f2a-9ebd-b1f8df9fef03", "Read a book", "Read a book everyday and write some notes about it.")]
    [InlineData("798a4706-4c51-48c9-8310-531d7364c926", "Have a walk", "Have a walk everyday for at least 20 mins.")]
    public async Task GetTaskById_ReturnTask_WhenTaskExists(Guid id, string title, string description)
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
        taskItem.Title.Should().Be(title);
        taskItem.Description.Should().Be(description);
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
        //Ensure API response contains expected data
        taskItem.Should().NotBeNull();
        taskItem.Id.Should().NotBeEmpty();
        taskItem.Title.Should().Be(testTaskItem.Title);
        taskItem.Description.Should().Be(testTaskItem.Description);
        //Verify data in database
        using var scope = factory.Services.CreateScope();
        var scopedService = scope.ServiceProvider;
        var db = scopedService.GetRequiredService<ApplicationDbContext>();
        var taskItemInDb = await db.TaskItems.FindAsync(taskItem.Id);
        taskItemInDb.Should().NotBeNull();
        taskItemInDb.Title.Should().Be(taskItem.Title);
        taskItemInDb.Description.Should().Be(taskItem.Description);
        //Clean up the database
        Utilities.Reset(db);
    }

    [Fact]
    public async Task CreateTask_ReturnsBadRequest_WhenInValidInput()
    {
        //Arrange
        var client = factory.CreateClient();
        var taskItem = new TaskItem
        {
            Description = "Test task item without required title property"
        };
        // var json = JsonSerializer.Serialize(taskItem);
        // var data = new StringContent(json, Encoding.UTF8, "application/json");
        //Act
        var response = await client.PostAsJsonAsync("/api/taskItems", taskItem);
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        //Serialize
        // var json = JsonSerializer.Serialize(testUpdateTaskItem);
        // var data = new StringContent(json, Encoding.UTF8, "application/json");
        //Act
        var response = await client.PutAsJsonAsync($"/api/taskItems/{id}", testUpdateTaskItem);
        //Assert
        //HTTP Response
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        //Deserialize
        var responseContent = await response.Content.ReadAsStringAsync();
        var taskItem = JsonSerializer.Deserialize<TaskItem>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        //Ensure API response contains expected data:
        taskItem.Should().NotBeNull();
        taskItem.Id.Should().NotBeEmpty();
        taskItem.Title.Should().Be(testUpdateTaskItem.Title);
        taskItem.Description.Should().Be(testUpdateTaskItem.Description);
        //Verify data in Database
        using var scope = factory.Services.CreateScope();
        var scopedService = scope.ServiceProvider;
        var db = scopedService.GetRequiredService<ApplicationDbContext>();
        // var taskInDb = await db.TaskItems.FindAsync(taskItem.Id);
        var taskInDb = await db.TaskItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == taskItem.Id);
        taskInDb.Should().NotBeNull();
        taskInDb.Description.Should().Be(taskItem.Description);
        taskInDb.Title.Should().Be(taskItem.Title);
        //Clean up the database
        Utilities.Reset(db);
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
    public async Task UpdateTask_ReturnsBadRequest_WhenInvalidInput()
    {
        //Arrange
        var client = factory.CreateClient();
        var taskItem = new TaskItem
        {
            Title = ""
        };
        // var json = JsonSerializer.Serialize(taskItem);
        // var data = new StringContent(json, Encoding.UTF8, "application/json");
        //Act
        var response = await client.PutAsJsonAsync("/api/taskItems/8a9de219-2dde-4f2a-9ebd-b1f8df9fef03", taskItem);
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deletedTask = await db.TaskItems.FindAsync(id);
        deletedTask.Should().BeNull();
        Utilities.Reset(db);
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
