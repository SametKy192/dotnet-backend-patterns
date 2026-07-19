using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests.Tests;

public class DomainAndApplicationTests
{
    private static readonly string ApplicationNamespace = "ArchitectureTests.Application";
    private static readonly string InfrastructureNamespace = "ArchitectureTests.Infrastructure";
    private static readonly string ApiNamespace = "ArchitectureTests.Api";


    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Entities.Product).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Domain layer should not depend on other layers.");
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnInfrastructureOrApi()
    {
        // Arrange
        var applicationAssembly = typeof(Application.IAssemblyMarker).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Application layer should not depend on Infrastructure or Api layers.");
    }
}
