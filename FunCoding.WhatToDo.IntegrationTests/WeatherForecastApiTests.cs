using FluentAssertions;
using FunCoding.WhatToDo.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace FunCoding.WhatToDo.IntegrationTests;

public class WeatherForecastApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetWeatherForecast_ReturnsSuccessAndCorrectContentType()
    {
        //arrange
        var client = factory.CreateClient();
        //Act
        var response = await client.GetAsync("/WeatherForecast");
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        //Deserialize the response
        var responseContent = await response.Content.ReadAsStringAsync();
        var weatherForecast = JsonSerializer.Deserialize<List<WeatherForecast>>(responseContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        weatherForecast.Should().NotBeNull();
        weatherForecast.Should().HaveCount(5);


    }

}
