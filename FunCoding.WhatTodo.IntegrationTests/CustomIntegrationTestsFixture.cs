using FunCoding.WhatTodo.IntegrationTests.Helpers;
using FunCoding.WhatToDo.WebApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FunCoding.WhatTodo.IntegrationTests;

public class CustomIntegrationTestsFixture : WebApplicationFactory<Program>
{
    private const string ConnectionString =
        "Server=localhost,1433;Database=WhatToDoServerTest;User Id=SA;Password=Burn@123;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set up test database
        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });
            using var scope = services.BuildServiceProvider().CreateScope();
            var scopeServices = scope.ServiceProvider;
            var dbContext = scopeServices.GetRequiredService<ApplicationDbContext>();
            Utilities.InitializeDatabase(dbContext);


        });
    }

}
