using Testcontainers.PostgreSql;
using Xunit;

namespace IntegrationTests.Tests;

public class ProductApiTests : IAsyncLifetime
{
    // Testcontainer instance for PostgreSQL
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        // Start the Docker container
        await _dbContainer.StartAsync();
    }

    [Fact]
    public void TestContainer_ShouldStartSuccessfully()
    {
        // Assert container status
        Assert.NotNull(_dbContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        // Stop and clean up the container
        await _dbContainer.StopAsync();
    }
}
