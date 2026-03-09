using Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Api.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="ServiceCollectionExtensions"/>.
/// Verifies the public contract of <see cref="ServiceCollectionExtensions.AddApplicationServices"/>:
/// the method must return the same <see cref="IServiceCollection"/> instance it received
/// (enabling method chaining) and must not throw under any circumstances.
/// </summary>
public sealed class ServiceCollectionExtensionsTests
{
    #region AddApplicationServices Tests

    [Fact]
    public void AddApplicationServices_WhenCalled_ReturnsSameServiceCollectionInstance()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddApplicationServices_WhenCalled_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var thrownException = Record.Exception(() => services.AddApplicationServices());

        // Assert
        Assert.Null(thrownException);
    }

    #endregion
}
