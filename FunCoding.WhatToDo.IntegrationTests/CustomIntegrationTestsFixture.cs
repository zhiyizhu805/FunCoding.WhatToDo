using FunCoding.WhatToDo.IntegrationTests.Helpers;
using FunCoding.WhatToDo.WebApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FunCoding.WhatToDo.IntegrationTests;

public class CustomIntegrationTestsFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var basePath = AppContext.BaseDirectory;
            var integrationTestConfigPath = Path.Combine(basePath, "appsettings.IntegrationTests.json");
            config.AddJsonFile(integrationTestConfigPath, optional: false, reloadOnChange: true)
                .AddUserSecrets<CustomIntegrationTestsFixture>(optional: true, reloadOnChange: true);
        });
        builder.ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            using var scope = services.BuildServiceProvider().CreateScope();
            var scopeServices = scope.ServiceProvider;
            var dbContext = scopeServices.GetRequiredService<ApplicationDbContext>();
            Utilities.InitializeDatabase(dbContext);
        });
    }

}
