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

}
