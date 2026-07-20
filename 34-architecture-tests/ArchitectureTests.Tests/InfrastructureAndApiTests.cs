using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests.Tests;

public class InfrastructureAndApiTests
{
    private static readonly string ApiNamespace = "ArchitectureTests.Api";


    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnApi()
    {
        // Arrange
        var infrastructureAssembly = typeof(Infrastructure.IAssemblyMarker).Assembly;

        var otherProjects = new[]
        {
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Infrastructure layer should not depend on Api layer.");
    }

    [Fact]
    public void Controllers_Should_InheritFromControllerBase()
    {
        // Arrange
        var apiAssembly = typeof(Api.Controllers.ProductsController).Assembly;

        // Act
        var result = Types.InAssembly(apiAssembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All controllers must inherit from ControllerBase.");
    }

    [Fact]
    public void Controllers_Should_HaveControllerSuffix()
    {
        // Arrange
        var apiAssembly = typeof(Api.Controllers.ProductsController).Assembly;

        // Act
        var result = Types.InAssembly(apiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All classes inheriting from ControllerBase must have the 'Controller' suffix.");
    }
}
