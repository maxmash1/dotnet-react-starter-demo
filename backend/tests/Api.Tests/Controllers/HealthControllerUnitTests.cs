using Api.Controllers;
using Api.DTOs.Common;
using Api.DTOs.Health;
using Api.Tests.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace Api.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="HealthController"/>.
/// Isolates the controller by mocking <see cref="IWebHostEnvironment"/> and
/// wiring a <see cref="DefaultHttpContext"/> directly — no HTTP server required.
/// </summary>
public sealed class HealthControllerUnitTests
{
    private readonly Mock<IWebHostEnvironment> _mockHostEnvironment;
    private readonly HealthController _controller;
    private readonly DefaultHttpContext _httpContext;

    /// <summary>
    /// Initializes shared test infrastructure: a mocked environment returning
    /// <c>"Testing"</c> and a controller wired to a <see cref="DefaultHttpContext"/>
    /// with a predictable request URL.
    /// </summary>
    public HealthControllerUnitTests()
    {
        _mockHostEnvironment = new Mock<IWebHostEnvironment>();
        _mockHostEnvironment.Setup(e => e.EnvironmentName).Returns("Testing");

        _controller = new HealthController(_mockHostEnvironment.Object);

        _httpContext = new DefaultHttpContext();
        _httpContext.Request.Scheme = "http";
        _httpContext.Request.Host = new HostString("localhost");
        _httpContext.Request.PathBase = PathString.Empty;
        _httpContext.Request.Path = "/v1/health";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    #region GetHealth — HTTP Result Type Tests

    [Fact]
    public void GetHealth_WhenCalled_ReturnsOkObjectResult()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public void GetHealth_WhenCalled_ReturnsItemResponseDtoEnvelope()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.IsType<ItemResponseDto<HealthResponseDto>>(okResult.Value);
    }

    #endregion

    #region GetHealth — Item Payload Tests

    [Fact]
    public void GetHealth_WhenCalled_ItemStatusIsHealthy()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Equal("healthy", envelope.Item.Status);
    }

    [Fact]
    public void GetHealth_WhenCalled_ItemEnvironmentMatchesMockedHostEnvironment()
    {
        // Arrange
        _mockHostEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Equal("Production", envelope.Item.Environment);
    }

    [Fact]
    public void GetHealth_WhenCalled_ItemVersionIsNotNullOrEmpty()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Item.Version),
            "Version must be populated from assembly metadata or fall back to '1.0.0'.");
    }

    [Fact]
    public void GetHealth_WhenCalled_ItemCheckedAtDateIsPopulatedWithUtcTime()
    {
        // Arrange
        var beforeCall = DateTime.UtcNow;

        // Act
        var actionResult = _controller.GetHealth();
        var afterCall = DateTime.UtcNow;

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.True(envelope.Item.CheckedAtDate >= beforeCall,
            "CheckedAtDate must not be earlier than the time just before the call.");
        Assert.True(envelope.Item.CheckedAtDate <= afterCall,
            "CheckedAtDate must not be later than the time just after the call.");
    }

    #endregion

    #region GetHealth — Metadata Tests

    [Fact]
    public void GetHealth_WhenTransactionIdPresentInHttpContextItems_UsesExistingTransactionId()
    {
        // Arrange
        var knownTransactionId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee";
        _httpContext.Items["TransactionId"] = knownTransactionId;

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Equal(knownTransactionId, envelope.Metadata.TransactionId);
    }

    [Fact]
    public void GetHealth_WhenTransactionIdAbsentFromHttpContextItems_FallsBackToNewGuid()
    {
        // Arrange — Items["TransactionId"] is deliberately NOT set

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Metadata.TransactionId),
            "TransactionId must be populated with a new GUID when not present in HttpContext.Items.");
        Assert.True(Guid.TryParse(envelope.Metadata.TransactionId, out _),
            $"Fallback TransactionId '{envelope.Metadata.TransactionId}' must be a valid GUID.");
    }

    [Fact]
    public void GetHealth_WhenCalled_MetadataTotalCountIsNull()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Null(envelope.Metadata.TotalCount);
    }

    [Fact]
    public void GetHealth_WhenCalled_MetadataTimestampIsPopulatedWithUtcTime()
    {
        // Arrange
        var beforeCall = DateTime.UtcNow;

        // Act
        var actionResult = _controller.GetHealth();
        var afterCall = DateTime.UtcNow;

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.True(envelope.Metadata.Timestamp >= beforeCall,
            "Metadata.Timestamp must not be earlier than the time just before the call.");
        Assert.True(envelope.Metadata.Timestamp <= afterCall,
            "Metadata.Timestamp must not be later than the time just after the call.");
    }

    #endregion

    #region GetHealth — Links Tests

    [Fact]
    public void GetHealth_WhenCalled_LinksSelfContainsRequestPath()
    {
        // Arrange — request path set to "/v1/health" in constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Contains("/v1/health", envelope.Links.Self);
    }

    [Fact]
    public void GetHealth_WhenCalled_LinksSelfIsConstructedFromRequestSchemeAndHost()
    {
        // Arrange
        _httpContext.Request.Scheme = "https";
        _httpContext.Request.Host = new HostString("api.example.com");
        _httpContext.Request.Path = "/v1/health";

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Equal("https://api.example.com/v1/health", envelope.Links.Self);
    }

    [Fact]
    public void GetHealth_WhenCalled_LinksNextIsNull()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Null(envelope.Links.Next);
    }

    [Fact]
    public void GetHealth_WhenCalled_LinksPrevIsNull()
    {
        // Arrange — controller ready via constructor

        // Act
        var actionResult = _controller.GetHealth();

        // Assert
        var envelope = ExtractEnvelope(actionResult);
        Assert.Null(envelope.Links.Prev);
    }

    #endregion

    #region GetHealth — Envelope Helpers via TestDataBuilders

    [Fact]
    public void GetHealth_WhenCalled_EnvelopeMatchesItemResponseDtoBuiltByTestDataBuilders()
    {
        // Arrange
        var expectedPayload = TestDataBuilders.BuildHealthResponse(
            status: "healthy",
            environment: "Testing");
        var expectedMetadata = TestDataBuilders.BuildMetadata();
        var expectedLinks = TestDataBuilders.BuildLinks(selfUrl: "http://localhost/v1/health");

        var builtEnvelope = TestDataBuilders.BuildItemResponse(expectedPayload, expectedMetadata, expectedLinks);

        // Act
        var actionResult = _controller.GetHealth();
        var actualEnvelope = ExtractEnvelope(actionResult);

        // Assert — structural shape only (timestamps/IDs will differ)
        Assert.NotNull(builtEnvelope.Item);
        Assert.NotNull(builtEnvelope.Metadata);
        Assert.NotNull(builtEnvelope.Links);
        Assert.Equal("healthy", actualEnvelope.Item.Status);
        Assert.Equal(builtEnvelope.Item.Status, actualEnvelope.Item.Status);
    }

    #endregion

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Unwraps an <see cref="ActionResult{T}"/> to the inner
    /// <see cref="ItemResponseDto{HealthResponseDto}"/>, failing the test with a
    /// descriptive message if the result shape is unexpected.
    /// </summary>
    private static ItemResponseDto<HealthResponseDto> ExtractEnvelope(
        ActionResult<ItemResponseDto<HealthResponseDto>> actionResult)
    {
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        return Assert.IsType<ItemResponseDto<HealthResponseDto>>(okResult.Value);
    }
}
