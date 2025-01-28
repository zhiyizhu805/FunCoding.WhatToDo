using System.Net;
using System.Text.Json;
using FluentAssertions;
using FunCoding.WhatToDo.WebApi.Models;

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



}
