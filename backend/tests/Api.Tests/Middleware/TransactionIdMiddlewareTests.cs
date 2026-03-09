using Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Xunit;

namespace Api.Tests.Middleware;

/// <summary>
/// Unit tests for <see cref="TransactionIdMiddleware"/>.
/// Verifies transaction ID generation, propagation to <c>HttpContext.Items</c>,
/// injection into response headers via the <c>OnStarting</c> callback, and
/// correct delegation to the next middleware in the pipeline.
/// </summary>
public sealed class TransactionIdMiddlewareTests
{
    #region InvokeAsync — HttpContext.Items Tests

    [Fact]
    public async Task InvokeAsync_WhenCalled_SetsTransactionIdInHttpContextItems()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(context.Items.ContainsKey("TransactionId"),
            "HttpContext.Items must contain the 'TransactionId' key after middleware executes.");
        Assert.NotNull(context.Items["TransactionId"]);
    }

    [Fact]
    public async Task InvokeAsync_WhenCalled_TransactionIdInItemsIsValidGuidFormat()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var transactionId = context.Items["TransactionId"]?.ToString();
        Assert.NotNull(transactionId);
        Assert.True(Guid.TryParse(transactionId, out _),
            $"Transaction ID '{transactionId}' must be a valid GUID.");
    }

    [Fact]
    public async Task InvokeAsync_WhenCalled_TransactionIdUsesHyphenatedGuidFormat()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        // "D" format produces 32 hex digits separated by 4 hyphens: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        var transactionId = context.Items["TransactionId"]?.ToString();
        Assert.NotNull(transactionId);
        Assert.Equal(36, transactionId.Length);
        Assert.Equal(4, transactionId.Count(c => c == '-'));
    }

    #endregion

    #region InvokeAsync — Next Delegate Tests

    [Fact]
    public async Task InvokeAsync_WhenCalled_CallsNextMiddlewareDelegateExactlyOnce()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCallCount = 0;
        RequestDelegate next = _ =>
        {
            nextCallCount++;
            return Task.CompletedTask;
        };
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(1, nextCallCount);
    }

    #endregion

    #region InvokeAsync — Uniqueness Tests

    [Fact]
    public async Task InvokeAsync_CalledMultipleTimes_EachRequestGetsUniqueTransactionId()
    {
        // Arrange
        var firstContext = new DefaultHttpContext();
        var secondContext = new DefaultHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(firstContext);
        await middleware.InvokeAsync(secondContext);

        // Assert
        var firstId = firstContext.Items["TransactionId"]?.ToString();
        var secondId = secondContext.Items["TransactionId"]?.ToString();
        Assert.NotNull(firstId);
        Assert.NotNull(secondId);
        Assert.NotEqual(firstId, secondId);
    }

    #endregion

    #region InvokeAsync — Response Header Tests

    [Fact]
    public async Task InvokeAsync_WhenResponseStarts_AddsXTransactionIdResponseHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseFeature = new TestHttpResponseFeature();
        context.Features.Set<IHttpResponseFeature>(responseFeature);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingCallbacksAsync();

        // Assert
        Assert.True(responseFeature.Headers.ContainsKey("X-Transaction-Id"),
            "Response headers must include 'X-Transaction-Id' after OnStarting fires.");
    }

    [Fact]
    public async Task InvokeAsync_WhenResponseStarts_ResponseHeaderValueIsValidGuid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseFeature = new TestHttpResponseFeature();
        context.Features.Set<IHttpResponseFeature>(responseFeature);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingCallbacksAsync();

        // Assert
        var headerValue = responseFeature.Headers["X-Transaction-Id"].ToString();
        Assert.NotEmpty(headerValue);
        Assert.True(Guid.TryParse(headerValue, out _),
            $"X-Transaction-Id header value '{headerValue}' must be a valid GUID.");
    }

    [Fact]
    public async Task InvokeAsync_WhenResponseStarts_HeaderValueMatchesContextItemTransactionId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseFeature = new TestHttpResponseFeature();
        context.Features.Set<IHttpResponseFeature>(responseFeature);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new TransactionIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingCallbacksAsync();

        // Assert
        var contextTransactionId = context.Items["TransactionId"]?.ToString();
        var headerTransactionId = responseFeature.Headers["X-Transaction-Id"].ToString();
        Assert.NotNull(contextTransactionId);
        Assert.Equal(contextTransactionId, headerTransactionId);
    }

    #endregion

    /// <summary>
    /// Test-only implementation of <see cref="IHttpResponseFeature"/> that captures
    /// callbacks registered via <see cref="OnStarting"/> and exposes
    /// <see cref="FireOnStartingCallbacksAsync"/> to simulate the server firing them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="DefaultHttpContext"/> does not automatically invoke <c>OnStarting</c>
    /// callbacks because there is no real Kestrel pipeline to wire them up. This helper
    /// replaces the default <see cref="IHttpResponseFeature"/> in the context's feature
    /// collection so that tests can deterministically trigger those callbacks and then
    /// inspect the resulting response headers.
    /// </para>
    /// <para>
    /// Because <c>DefaultHttpResponse.Headers</c> reads from
    /// <c>context.Features.Get&lt;IHttpResponseFeature&gt;().Headers</c>, replacing the
    /// feature also redirects all header reads/writes through this instance's
    /// <see cref="Headers"/> dictionary — keeping assertions consistent.
    /// </para>
    /// </remarks>
    private sealed class TestHttpResponseFeature : IHttpResponseFeature
    {
        private readonly List<(Func<object, Task> Callback, object State)> _onStartingCallbacks = [];

        /// <inheritdoc />
        public int StatusCode { get; set; } = 200;

        /// <inheritdoc />
        public string? ReasonPhrase { get; set; }

        /// <inheritdoc />
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

        /// <inheritdoc />
        public Stream Body { get; set; } = Stream.Null;

        /// <inheritdoc />
        public bool HasStarted { get; private set; }

        /// <inheritdoc />
        public void OnStarting(Func<object, Task> callback, object state)
            => _onStartingCallbacks.Add((callback, state));

        /// <inheritdoc />
        public void OnCompleted(Func<object, Task> callback, object state) { }

        /// <summary>
        /// Simulates the server invoking all registered <c>OnStarting</c> callbacks
        /// in the order they were registered.
        /// </summary>
        public async Task FireOnStartingCallbacksAsync()
        {
            HasStarted = true;
            foreach (var (callback, state) in _onStartingCallbacks)
            {
                await callback(state);
            }
        }
    }
}
